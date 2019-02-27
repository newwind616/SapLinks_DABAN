﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DSCW
{
    public class DSCW_Action : CompanyObject, ICompanyAction
    {
        public DSCW_Action()
        {
            company = "DSCW";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DSCW_Path_P".ToAppSetting();
            _fileName = "DSCW_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DSCW_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DSCW = D_DSCWD();
            //文件拼接
            string fileData = DSCW.file_sb.ToString();
            //脚本拼接
            string sql = DSCW.upLinks_sql.ToString();
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
        /// DSCW当日往返申请
        /// </summary>
        private DataConvert D_DSCWD()
        {
            LogInfo.Log.Info("《DSCW个人经费》数据装载");
            DataConvert entity = new DSCWD(this);
            entity.GetData();
            return entity;
        }
    }
}