using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 发票类型
    /// </summary>
    public class InvoiceType
    {
        public const string SpecialInvoice = "ZP";
        public const string PlainInvoice = "PP";
        public string _Invoice;
        public InvoiceType(string invoiceType)
        {
            this._Invoice = invoiceType;
        }
        /// <summary>
        /// 是否专票
        /// </summary>
        public bool IsSpecialInvoice
        {
            get
            {
                if (_Invoice == InvoiceType.SpecialInvoice)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否普票
        /// </summary>
        public bool IsPlainInvoice
        {
            get
            {
                if (_Invoice == InvoiceType.PlainInvoice)
                    return true;
                else
                    return false;
            }
        }
    }
}
