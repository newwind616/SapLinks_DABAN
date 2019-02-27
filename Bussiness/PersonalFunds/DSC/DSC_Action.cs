using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DSC
{
    public class DSC_Action : CompanyObject, ICompanyAction
    {
        public DSC_Action()
        {
            company = "DSC";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DSC_Path_P".ToAppSetting();
            _fileName = "DSC_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DSC_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DSC = D_DSCD();
            //文件拼接
            string fileData = DSC.file_sb.ToString();
            //脚本拼接
            string sql = DSC.upLinks_sql.ToString();
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
        /// DSC当日往返申请
        /// </summary>
        private DataConvert D_DSCD()
        {
            LogInfo.Log.Info("《DSC个人经费》数据装载");
            DataConvert entity = new DSCD(this);
            entity.GetData();
            return entity;
        }
    }
}
