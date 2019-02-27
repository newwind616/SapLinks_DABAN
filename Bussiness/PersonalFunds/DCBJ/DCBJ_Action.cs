using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DCBJ
{
    public class DCBJ_Action : CompanyObject, ICompanyAction
    {
        public DCBJ_Action()
        {
            company = "DCBJ";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DCBJ_Path_P".ToAppSetting();
            _fileName = "DCBJ_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DCBJ_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DCBJ = D_DCBJD();
            //文件拼接
            string fileData = DCBJ.file_sb.ToString();
            //脚本拼接
            string sql = DCBJ.upLinks_sql.ToString();
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
        /// DCBJ当日往返申请
        /// </summary>
        private DataConvert D_DCBJD()
        {
            LogInfo.Log.Info("《DCBJ个人经费》数据装载");
            DataConvert entity = new DCBJD(this);
            entity.GetData();
            return entity;
        }
    }
}
