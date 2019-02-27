using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;

namespace SAPLinks
{
    public class SQLHelperExt
    {
        public static object ExecuteFirstValue(string connectionString, string commandText)
        {
            return ExecuteAssignValue(connectionString, commandText, 0, 0);
        }
        public static object ExecuteAssignValue(string connectionString, string commandText,int rowIndex,int columnIndex)
        {
            DataTable dt = SQLHelper.ExecuteDataset(connectionString, CommandType.Text, commandText).Tables[0];
            if (dt.Rows.Count == 0)
                return null;
            else
                return dt.Rows[0][0];
        }
    }
}
