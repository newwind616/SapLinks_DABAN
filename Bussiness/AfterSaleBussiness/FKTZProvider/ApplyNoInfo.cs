using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 开票申请与付款通知书关联
    /// </summary>
    public class ApplyNoInfo
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string ApplyNo { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 作业单
        /// </summary>
        public string CBXZ { get; set; }
        /// <summary>
        /// 当前作业单在对应TaskID中
        /// </summary>
        public string CurrentTaskId { get; set; }
        /// <summary>
        /// 当前作业单的开票申请所在TaskID
        /// </summary>
        public string SrcTaskId { get; set; }

    }
}
