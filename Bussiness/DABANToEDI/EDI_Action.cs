using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.DABANToEDI
{
    public class EDI_Action : EDIObject, ICompanyAction
    {

        public EDI_Action()
        {
            _filePath = "MAIN_EDIToDABAN_Path".ToAppSetting();
            _fileName = "MAIN_EDIToDABAN_Name".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "MAIN_EDIToDABAN_Ext".ToAppSetting();
        }

        public void Start()
        {
            DataConvert EDI = E_EDI();
            //文件拼接
            string fileData = EDI.file_sb.ToString();
            //脚本拼接
            string sql = EDI.upLinks_sql.ToString();
            if (string.IsNullOrEmpty(sql))
            {
                MainFile.WriteFile_(filePath, fileName, fileData);
                return;
            }
            SqlCommand cmd = SQLHelper.GetTransactionSqlCommand(connStr);
            SQLHelper.ExecuteNonQuery(ref cmd, sql);
            if (MainFile.WriteFile_(filePath, fileName, fileData))
                cmd.Transaction.Commit();
            else
                cmd.Transaction.Rollback();
            cmd.Connection.Close();

        }

        private DataConvert E_EDI()
        {
            LogInfo.Log.Info("《DABANToEDI》数据装载");
            DataConvert entity = new EDI(this);
            entity.GetData();
            return entity;
        }

    }
}
