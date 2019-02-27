using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DCT
{
    public class DCT_Action : CompanyObject, ICompanyAction
    {
        public DCT_Action()
        {
            company = "DCT";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DCT_Path_P".ToAppSetting();
            _fileName = "DCT_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DCT_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DCT = D_DCT();
            //文件拼接
            string fileData = DCT.file_sb.ToString();
            //脚本拼接
            string sql = DCT.upLinks_sql.ToString();
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
        /// DCT当日往返申请
        /// </summary>
        private DataConvert D_DCT()
        {
            LogInfo.Log.Info("《DCT个人经费》数据装载");
            DataConvert entity = new DCT(this);
            entity.GetData();
            return entity;
        }
    }
}
