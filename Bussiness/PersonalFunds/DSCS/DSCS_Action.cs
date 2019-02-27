using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DSCS
{
    public class DSCS_Action : CompanyObject, ICompanyAction
    {
        public DSCS_Action()
        {
            company = "DSCS";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DSCS_Path_P".ToAppSetting();
            _fileName = "DSCS_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DSCS_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DSCS = D_DSCSD();
            //文件拼接
            string fileData = DSCS.file_sb.ToString();
            //脚本拼接
            string sql = DSCS.upLinks_sql.ToString();
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
        /// DSCS当日往返申请
        /// </summary>
        private DataConvert D_DSCSD()
        {
            LogInfo.Log.Info("《DSCS个人经费》数据装载");
            DataConvert entity = new DSCSD(this);
            entity.GetData();
            return entity;
        }
    }
}
