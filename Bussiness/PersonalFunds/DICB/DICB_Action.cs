using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DICB
{
    public class DICB_Action : CompanyObject, ICompanyAction
    {
        public DICB_Action()
        {
            company = "DICB";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DICB_Path_P".ToAppSetting();
            _fileName = "DICB_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DICB_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DICB = D_DICB();
            //文件拼接
            string fileData = DICB.file_sb.ToString();
            //脚本拼接
            string sql = DICB.upLinks_sql.ToString();
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
        /// DICB当日往返申请
        /// </summary>
        private DataConvert D_DICB()
        {
            LogInfo.Log.Info("《DICB个人经费》数据装载");
            DataConvert entity = new DICB(this);
            entity.GetData();
            return entity;
        }
    }
}
