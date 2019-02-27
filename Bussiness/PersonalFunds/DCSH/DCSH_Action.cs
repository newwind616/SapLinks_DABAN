using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DCSH
{
    public class DCSH_Action : CompanyObject, ICompanyAction
    {
        public DCSH_Action()
        {
            company = "DCSH";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DCSH_Path_P".ToAppSetting();
            _fileName = "DCSH_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DCSH_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DCSH = D_DCSH();
            //文件拼接
            string fileData = DCSH.file_sb.ToString();
            //脚本拼接
            string sql = DCSH.upLinks_sql.ToString();
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
        /// DCSH当日往返申请
        /// </summary>
        private DataConvert D_DCSH()
        {
            LogInfo.Log.Info("《DCSH个人经费》数据装载");
            DataConvert entity = new DCSH(this);
            entity.GetData();
            return entity;
        }
    }
}
