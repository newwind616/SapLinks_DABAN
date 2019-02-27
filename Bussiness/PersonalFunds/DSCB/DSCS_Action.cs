using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DSCB
{
    public class DSCB_Action : CompanyObject, ICompanyAction
    {
        public DSCB_Action()
        {
            company = "DSCB";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DSCB_Path_P".ToAppSetting();
            _fileName = "DSCB_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DSCB_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DSCB = D_DSCBD();
            //文件拼接
            string fileData = DSCB.file_sb.ToString();
            //脚本拼接
            string sql = DSCB.upLinks_sql.ToString();
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
        /// DSCB当日往返申请
        /// </summary>
        private DataConvert D_DSCBD()
        {
            LogInfo.Log.Info("《DSCB个人经费》数据装载");
            DataConvert entity = new DSCBD(this);
            entity.GetData();
            return entity;
        }
    }
}
