using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 付款通知书流程类型
    /// </summary>
    public class FKTZSProcessType
    {
        /// <summary>
        /// 付款通知书支付
        /// </summary>
        public const string FKTZS_ZF = "0";
        /// <summary>
        /// 付款通知书暂收款
        /// </summary>
        public const string FKTZS_ZSK = "1";
        /// <summary>
        /// 付款通知书有偿
        /// </summary>
        public const string FKTZS_YC = "2";
        /// <summary>
        /// 付款通知书无偿
        /// </summary>
        public const string FKTZS_WC = "2";
        public string ProcessType { get; set; }
        public FKTZSProcessType(string processType)
        {
            this.ProcessType = processType;
        }
    }
}
