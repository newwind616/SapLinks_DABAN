using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DSC_B
{
    public class DSC_B_Action : CompanyObject, ICompanyAction
    {
        public DSC_B_Action()
        {
            company = "DSC_B";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DSC_B_Path_P".ToAppSetting();
            _fileName = "DSC_B_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DSC_B_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DSC_B = D_DSC_BD();
            //文件拼接
            string fileData = DSC_B.file_sb.ToString();
            //脚本拼接
            string sql = DSC_B.upLinks_sql.ToString();
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
        /// DSC_B当日往返申请
        /// </summary>
        private DataConvert D_DSC_BD()
        {
            LogInfo.Log.Info("《DSC_B个人经费》数据装载");
            DataConvert entity = new DSC_BD(this);
            entity.GetData();
            return entity;
        }
    }
}
