using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DCGZ
{
    public class DCGZ_Action : CompanyObject, ICompanyAction
    {
        public DCGZ_Action()
        {
            company = "DCGZ";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DCGZ_Path_P".ToAppSetting();
            _fileName = "DCGZ_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DCGZ_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert dcgzd = D_DCGZD();
            //文件拼接
            string fileData = dcgzd.file_sb.ToString();
            //脚本拼接
            string sql = dcgzd.upLinks_sql.ToString();
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
        /// DCGZ当日往返申请
        /// </summary>
        private DataConvert D_DCGZD()
        {
            LogInfo.Log.Info("《DCGZ个人经费》数据装载");
            DataConvert entity = new DCGZD(this);
            entity.GetData();
            return entity;
        }
    }
}
