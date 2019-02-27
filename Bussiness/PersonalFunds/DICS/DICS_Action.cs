using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DICS
{
    public class DICS_Action : CompanyObject, ICompanyAction
    {
        public DICS_Action()
        {
            company = "DICS";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DICS_Path_P".ToAppSetting();
            _fileName = "DICS_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DICS_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DICS = D_DICS();
            //文件拼接
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
        /// <summary>
        /// DICS当日往返申请
        /// </summary>
        private DataConvert D_DICS()
        {
            LogInfo.Log.Info("《DICS个人经费》数据装载");
            DataConvert entity = new DICS(this);
            entity.GetData();
            return entity;
        }
    }
}
