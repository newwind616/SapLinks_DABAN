using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult
{
    /// <summary>
    /// SAP回传类型
    /// </summary>
    public enum SAPResultType
    {
        /// <summary>
        /// 支付回传
        /// </summary>
        Pay,
        /// <summary>
        /// 凭证回传
        /// </summary>
        Account
    }
}
