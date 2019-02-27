using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DCSL
{
    public class DCSL_Action : CompanyObject, ICompanyAction
    {
        public DCSL_Action()
        {
            company = "DCSL";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DCSL_Path_P".ToAppSetting();
            _fileName = "DCSL_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DCSL_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DCSL = D_DCSLD();
            //文件拼接
            string fileData = DCSL.file_sb.ToString();
            //脚本拼接
            string sql = DCSL.upLinks_sql.ToString();
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
        /// DCSL当日往返申请
        /// </summary>
        private DataConvert D_DCSLD()
        {
            LogInfo.Log.Info("《DCSL个人经费》数据装载");
            DataConvert entity = new DCSLD(this);
            entity.GetData();
            return entity;
        }
    }
}
