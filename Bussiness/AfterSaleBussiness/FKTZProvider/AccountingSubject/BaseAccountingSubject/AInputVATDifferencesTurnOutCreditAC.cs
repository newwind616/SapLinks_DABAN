using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public abstract class AInputVATDifferencesTurnOutCreditAC : IAccountingSubject
    {
        protected FKTZSServiceEntity context;
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
            //return decimal.Round(context.Fktzs_C_HEntitys.SjjeTotal / (1 + context.InvoiceEntitys.TaxTate) - context.InvoiceEntitys.TaxTotal, 2);
            decimal result = decimal.Round(context.Fktzs_C_HEntitys.SjjeTotal / (1 + context.InvoiceEntitys.TaxTate) * context.InvoiceEntitys.TaxTate, 2) - context.InvoiceEntitys.TaxTotal;
            return result;
        }
        /// <summary>
        /// 实际支付金额/(1+税率）*税率-专票税率合计的差异额
        /// <0:50 >0:40
        /// </summary>
        /// <returns></returns>
        protected virtual string GetNEWBS()
        {
            if (GetWRBTR() > 0)
                return "50";
            else
                return "40";
        }
        protected abstract List<AccVouch> DoLoad();
    }
}
