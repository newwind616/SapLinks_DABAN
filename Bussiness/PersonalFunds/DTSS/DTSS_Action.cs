using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DTSS
{
    public class DTSS_Action : CompanyObject, ICompanyAction
    {
        public DTSS_Action()
        {
            company = "DTSS";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DTSS_Path_P".ToAppSetting();
            _fileName = "DTSS_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DTSS_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DTSS = D_DTSSD();
            //文件拼接
            string fileData = DTSS.file_sb.ToString();
            //脚本拼接
            string sql = DTSS.upLinks_sql.ToString();
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
        /// DTSS当日往返申请
        /// </summary>
        private DataConvert D_DTSSD()
        {
            LogInfo.Log.Info("《DTSS个人经费》数据装载");
            DataConvert entity = new DTSSD(this);
            entity.GetData();
            return entity;
        }
    }
}
