using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.PersonalFunds
{
    public class CompanyObject:BaseAction
    {
        public string company;
        public string _fileName;
        public string _filePath;
        public string _fileExt;
        public int systype;
        protected string filePath
        {
            get
            {
                if (_filePath.LastIndexOf('\\') == _filePath.Length - 1)
                    return _filePath;
                else
                    return _filePath + "\\";
            }
        }
        protected string fileName { get { return _fileName + _fileExt; } }
        protected void FileAndDB(string sql,string fileData)
        {
            if (string.IsNullOrEmpty(sql))
            {
                MainFile.WriteFile(filePath, fileName, fileData);
                return;
            }
            System.Data.SqlClient.SqlCommand cmd = SQLHelper.GetTransactionSqlCommand(connStr);
            SQLHelper.ExecuteNonQuery(ref cmd, sql);
            if (MainFile.WriteFile(filePath, fileName, fileData))
                cmd.Transaction.Commit();
            else
                cmd.Transaction.Rollback();
            cmd.Connection.Close();
        }
    }
}
