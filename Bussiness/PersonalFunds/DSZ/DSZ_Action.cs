using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DSZ
{
    public class DSZ_Action : CompanyObject, ICompanyAction
    {
        public DSZ_Action()
        {
            company = "DSZ";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DSZ_Path_P".ToAppSetting();
            _fileName = "DSZ_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DSZ_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DSZ = D_DSZ();
            //文件拼接
            string fileData = DSZ.file_sb.ToString();
            //脚本拼接
            string sql = DSZ.upLinks_sql.ToString();
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
        /// DSZ当日往返申请
        /// </summary>
        private DataConvert D_DSZ()
        {
            LogInfo.Log.Info("《DSZ个人经费》数据装载");
            DataConvert entity = new DSZ(this);
            entity.GetData();
            return entity;
        }
    }
}
