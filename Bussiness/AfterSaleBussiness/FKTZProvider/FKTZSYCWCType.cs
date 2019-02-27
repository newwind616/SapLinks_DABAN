using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 付款通知书营业有偿/无偿
    /// </summary>
    public class FKTZSYCWCType
    {
        /// <summary>
        /// 有偿
        /// </summary>
        public const string YC = "0";
        /// <summary>
        /// 无偿
        /// </summary>
        public const string WC = "1";
        public string YCWCType { get; set; }
        public FKTZSYCWCType(string ycwc)
        {
            this.YCWCType = ycwc;
        }
    }
}
