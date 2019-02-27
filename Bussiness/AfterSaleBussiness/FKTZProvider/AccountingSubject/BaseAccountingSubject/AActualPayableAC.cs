using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public abstract class AActualPayableAC : IAccountingSubject
    {
        protected FKTZSServiceEntity context;
        protected DataTable dt;
        public List<AccVouch> Load(FKTZSServiceEntity fktzsServiceEntity)
        {
            context = fktzsServiceEntity;
            if (!Verification())
                return null;
            return DoLoad();
        }
        /// <summary>
        /// 1.必须是专票
        /// 2.实际支付金额/(1+税率）*税率-专票税额合计不等于0
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean Verification()
        {
            if (context.InvoiceEntitys.InvoiceType.IsSpecialInvoice && GetWRBTR() != 0)
                return true;
            return false;
        }
        /// <summary>
        /// 实际支付金额/(1+税率）*税率-专票税率合计的差异额
        /// </summary>
        /// <returns></returns>
        protected virtual decimal GetWRBTR()
        {
            decimal result = decimal.Round(context.Fktzs_C_HEntitys.SjjeTotal / (1 + context.InvoiceEntitys.TaxTate) * context.InvoiceEntitys.TaxTate, 2) - context.InvoiceEntitys.TaxTotal;
            return result;
        }
        protected abstract List<AccVouch> DoLoad();
    }
}
