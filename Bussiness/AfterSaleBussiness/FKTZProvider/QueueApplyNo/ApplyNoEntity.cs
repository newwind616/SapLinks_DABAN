using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 单号实体
    /// </summary>
    public class ApplyNoEntity
    {
        public string ApplyNo { get; set; }
        public string TaskID { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string FinishAt { get; set; }
        public ApplyNoBasicEntity BasicEntity { get; set; }
    }
}
