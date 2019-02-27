using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SAPLinks
{
    public class MSSQLFactory 
    {
        public static string DeleteBatch(DataTable dt)
        {
            return DeleteBatch(dt.PrimaryKey[0].ColumnName,dt);
        }
        /// <summary>
        /// 批量删除语句
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DeleteBatch(string key, DataTable dt)
        {
            int splitSize = 10000;//每次删除分割数量
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i % splitSize == 0)
                {
                    sb.Append(string.Format("delete from {0} where {1}='{2}' ", dt.TableName, key, dt.Rows[i][key].ToString().Replace("'","\"")));
                }
                else
                {
                    sb.Append(string.Format("or {0}='{1}' ", key, dt.Rows[i][key].ToString()));
                }
            }
            return sb.ToString();
        }

        public static void BulkCopy(string connString, DataTable dt)
        {
            BulkCopy(connString, dt.TableName, DeleteBatch(dt), dt);
        }
        /// <summary> 
        /// 大批量插入数据(2000每批次) 
        /// 已采用整体事物控制 
        /// </summary> 
        /// <param name="connString">数据库链接字符串</param>
        /// <param name="tableName">数据库服务器上目标表名</param> 
        /// <param name="dt">含有和目标数据库表结构完全一致(所包含的字段名完全一致即可)的DataTable</param> 
        public static void BulkCopy(string connString, string tableName, string deleteSql, DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandTimeout = 180;
                        command.Transaction = transaction;
                        command.Connection = conn;
                        command.CommandText = deleteSql;
                        command.ExecuteNonQuery();//执行sql语句
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.BatchSize = 2000;//每2000条发送一次
                            bulkCopy.BulkCopyTimeout = 30;//默认
                            bulkCopy.DestinationTableName = tableName;
                            try
                            {
                                foreach (DataColumn col in dt.Columns)
                                {
                                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                                }
                                bulkCopy.WriteToServer(dt);//执行插入
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                    }
                }
            }
        }
    }
}
