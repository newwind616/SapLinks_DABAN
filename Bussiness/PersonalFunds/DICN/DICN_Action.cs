using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DICN
{
    public class DICN_Action : CompanyObject, ICompanyAction
    {
        public DICN_Action()
        {
            company = "DICN";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DICN_Path_P".ToAppSetting();
            _fileName = "DICN_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DICN_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DICN = D_DICN();
            //文件拼接
            string fileData = DICN.file_sb.ToString();
            //脚本拼接
            string sql = DICN.upLinks_sql.ToString();
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
        /// DICN当日往返申请
        /// </summary>
        private DataConvert D_DICN()
        {
            LogInfo.Log.Info("《DICN个人经费》数据装载");
            DataConvert entity = new DICN(this);
            entity.GetData();
            return entity;
        }
    }
}
