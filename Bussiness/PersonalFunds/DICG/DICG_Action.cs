using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DICG
{
    public class DICG_Action : CompanyObject, ICompanyAction
    {
        public DICG_Action()
        {
            company = "DICG";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DICG_Path_P".ToAppSetting();
            _fileName = "DICG_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DICG_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DICG = D_DICG();
            //文件拼接
            string fileData = DICG.file_sb.ToString();
            //脚本拼接
            string sql = DICG.upLinks_sql.ToString();
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
        /// DICG当日往返申请
        /// </summary>
        private DataConvert D_DICG()
        {
            LogInfo.Log.Info("《DICG个人经费》数据装载");
            DataConvert entity = new DICG(this);
            entity.GetData();
            return entity;
        }
    }
}
