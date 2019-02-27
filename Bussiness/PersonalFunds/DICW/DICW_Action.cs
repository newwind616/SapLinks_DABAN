using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DICW
{
    public class DICW_Action : CompanyObject, ICompanyAction
    {
        public DICW_Action()
        {
            company = "DICW";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DICW_Path_P".ToAppSetting();
            _fileName = "DICW_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DICW_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DICW = D_DICW();
            //文件拼接
            string fileData = DICW.file_sb.ToString();
            //脚本拼接
            string sql = DICW.upLinks_sql.ToString();
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
        /// DICW当日往返申请
        /// </summary>
        private DataConvert D_DICW()
        {
            LogInfo.Log.Info("《DICW个人经费》数据装载");
            DataConvert entity = new DICW(this);
            entity.GetData();
            return entity;
        }
    }
}
