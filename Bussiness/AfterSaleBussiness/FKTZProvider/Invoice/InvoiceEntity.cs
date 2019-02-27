using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 发票信息实体
    /// </summary>
    public class InvoiceEntity
    {
        public InvoiceType InvoiceType { get; set; }
        /// <summary>
        /// 发票代码
        /// </summary>
        public string Inv_Code { get; set; }
        /// <summary>
        /// 发票号码
        /// </summary>
        public string Inv_No { get; set; }
        /// <summary>
        /// 发票金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 发票税额
        /// </summary>
        public decimal Tax { get; set; }
        /// <summary>
        /// 税率小数位0.16、0.11、0.03等
        /// </summary>
        public decimal TaxRate
        {
            get
            {
                if (InvoiceType.IsSpecialInvoice)
                    return decimal.Round(Tax / Amount, 2);
                else
                    return 0;
            }
        }
    }
}
