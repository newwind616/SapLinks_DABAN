using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Helper.SaveFile
{
    public class SapLinksQueueEntity
    {
        public string ConnStr { get; set; }
        public string UpdateSql { get; set; }
        public SapLinksQueueEntity(string connStr, string updateSql)
        {
            this.ConnStr = connStr;
            this.UpdateSql = updateSql;
        }
    }
}
