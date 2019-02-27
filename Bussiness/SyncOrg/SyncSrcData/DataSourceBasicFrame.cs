using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SyncOrg.SyncSrcData
{
    public class DataSourceBasicFrame
    {
        /// <summary>
        /// 创建数据源表框架
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual DataTable CreateDataTableFrame(DataSourceType type)
        {
            DataTable dt = new DataTable();
            if (type == DataSourceType.OU)
            {
                dt.Columns.Add("OUCode", typeof(string));
                dt.Columns.Add("OULevel", typeof(string));
                dt.Columns.Add("OUName", typeof(string));
                dt.Columns.Add("POUCode", typeof(string));
                dt.Columns.Add("POULevel", typeof(string));
                dt.Columns.Add("POUName", typeof(string));
            }
            if (type == DataSourceType.User)
            {
                dt.Columns.Add("Account", typeof(string));
                dt.Columns.Add("DisplayName", typeof(string));
                dt.Columns.Add("OUCode", typeof(string));
                dt.Columns.Add("OUName", typeof(string));
                dt.Columns.Add("SupervisorUserAccount", typeof(string));
                dt.Columns.Add("LeaderTitle", typeof(string));
                dt.Columns.Add("EMail", typeof(string));
                dt.Columns.Add("DateHired", typeof(string));
                dt.Columns.Add("Sex", typeof(string));
                dt.Columns.Add("Disabled", typeof(bool));
            }
            return dt;
        }
    }
}
