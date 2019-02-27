using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SAPLinks.Bussiness.PersonalFunds.DCMH
{
    public class DCMH_Action : CompanyObject, ICompanyAction
    {
        public DCMH_Action()
        {
            company = "DCMH";
            systype = 1;//0:公司经费 1:个人经费
            _filePath = "DCMH_Path_P".ToAppSetting();
            _fileName = "DCMH_Name_P".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "DCMH_Ext_P".ToAppSetting();
        }
        public void Start()
        {
            DataConvert DCMH = D_DCMHD();
            //文件拼接
            string fileData = DCMH.file_sb.ToString();
            //脚本拼接
            string sql = DCMH.upLinks_sql.ToString();
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
        /// DCMH当日往返申请
        /// </summary>
        private DataConvert D_DCMHD()
        {
            LogInfo.Log.Info("《DCMH个人经费》数据装载");
            DataConvert entity = new DCMHD(this);
            entity.GetData();
            return entity;
        }
    }
}
