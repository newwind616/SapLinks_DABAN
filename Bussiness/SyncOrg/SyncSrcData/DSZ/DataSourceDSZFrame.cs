using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SyncOrg.SyncSrcData.DSZ
{
    public class DataSourceDSZFrame : DataSourceBasicFrame
    {
        public override DataTable CreateDataTableFrame(DataSourceType type)
        {
            DataTable dt = base.CreateDataTableFrame(type);
            if (type == DataSourceType.User)
            {
                dt.Columns.Add("DEPT_TYPE", typeof(string));
                dt.Columns.Add("IN_DATE", typeof(DateTime));
            }
            return dt;
        }
    }
}
