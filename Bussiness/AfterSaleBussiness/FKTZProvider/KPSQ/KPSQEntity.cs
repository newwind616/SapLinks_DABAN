using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 开票申请明细实体
    /// </summary>
    public class KPSQEntity
    {
        /// <summary>
        /// 作业单号
        /// </summary>
        public string XBLNR { get; set; }
        /// <summary>
        /// 外包除税金额
        /// </summary>
        public decimal WBCS { get; set; }
    }
}
