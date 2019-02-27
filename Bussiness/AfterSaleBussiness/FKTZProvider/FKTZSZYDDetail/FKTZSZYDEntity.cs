using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 付款通知书明细单据
    /// </summary>
    public class FKTZSZYDEntity
    {
        public int ID { get; set; }
        public int TASKID { get; set; }
        public string Z_GLBH { get; set; }
        public string Z_GLBH_PK { get; set; }
        public string Z_YWLX { get; set; }
        public string Z_HTBH { get; set; }
        public string Z_YYDD { get; set; }
        public string KHBM { get; set; }
        public decimal S_KPJE { get; set; }
        public string C_SAKNR { get; set; }
        public string C_SNWBMC { get; set; }
        public decimal C_WBJE { get; set; }
        public decimal SJZFJE { get; set; }
    }
}
