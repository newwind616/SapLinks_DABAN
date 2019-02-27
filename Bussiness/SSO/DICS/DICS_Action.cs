using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.SSO.DICS
{
    public class DICS_Action : CompanyObject, ICompanyAction
    {
        public DICS_Action()
        {
           // company = "DICS";
           // systype = 0;//0:公司经费 1:个人经费
            _filePath = "DICS_Path_SSO".ToAppSetting();
            _fileName = "DICS_Name_SSO".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DICS_Ext_SSO".ToAppSetting();
        }

        public void Start()
        {
            DataConvert DICS = D_DICS();
            string fileData = DICS.file_sb.ToString();
            //脚本拼接
            string sql = DICS.upLinks_sql.ToString();

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

        private DataConvert D_DICS()
        {
            LogInfo.Log.Info("《DICS_SSO》数据装载");
            DataConvert entity = new DICS(this);
            entity.GetData();
            return entity;
        }
    }
}
