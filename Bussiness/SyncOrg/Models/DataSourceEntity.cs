using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SyncOrg
{
    public class DataSourceEntity
    {
        public DataSourceEntity() { }
        public DataSourceType Type { get; set; }

    }
    public enum DataSourceType
    {
        OU,
        User
    }
}
