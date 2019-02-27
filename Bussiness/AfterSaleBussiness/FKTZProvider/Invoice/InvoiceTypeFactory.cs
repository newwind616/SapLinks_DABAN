using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 发票类型工厂
    /// </summary>
    public class InvoiceTypeFactory
    {
        public static InvoiceType Init(string invoiceType)
        {
            if (invoiceType == "ZP")
                return new InvoiceType(InvoiceType.SpecialInvoice);
            if (invoiceType == "PP")
                return new InvoiceType(InvoiceType.PlainInvoice);
            return null;
        }
    }
}
