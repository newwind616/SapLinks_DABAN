using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DFS
{
    public class DFS_Action : CompanyObject, ICompanyAction
    {
        public DFS_Action()
        {
            company = "DFS";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DFS_Path_P".ToAppSetting();
            _fileName = "DFS_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DFS_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DFS = D_DFS();
            //文件拼接
            string fileData = DFS.file_sb.ToString();
            //脚本拼接
            string sql = DFS.upLinks_sql.ToString();
            if (string.IsNullOrEmpty(sql))
            {
                MainFile.WriteFile(filePath, fileName, fileData);
                return;
            }
            SqlCommand cmd = SQLHelper.GetTransactionSqlCommand(connStr);
            SQLHelper.ExecuteNonQuery(ref cmd, sql);
            if (MainFile.WriteFile(filePath, fileName, fileData))
                cmd.Transaction.Commit();
            else
                cmd.Transaction.Rollback();
            cmd.Connection.Close();
        }
        /// <summary>
        /// DFS当日往返申请
        /// </summary>
        private DataConvert D_DFS()
        {
            LogInfo.Log.Info("《DFS个人经费》数据装载");
            DataConvert entity = new DFS(this);
            entity.GetData();
            return entity;
        }
    }
}
