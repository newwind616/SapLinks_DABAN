using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DSCG
{
    public class DSCG_Action : CompanyObject, ICompanyAction
    {
        public DSCG_Action()
        {
            company = "DSCG";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DSCG_Path_P".ToAppSetting();
            _fileName = "DSCG_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DSCG_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DSCG = D_DSCG();
            //文件拼接
            string fileData = DSCG.file_sb.ToString();
            //脚本拼接
            string sql = DSCG.upLinks_sql.ToString();
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
        /// DSCG当日往返申请
        /// </summary>
        private DataConvert D_DSCG()
        {
            LogInfo.Log.Info("《DSCG个人经费》数据装载");
            DataConvert entity = new DSCG(this);
            entity.GetData();
            return entity;
        }
    }
}
