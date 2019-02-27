using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public abstract class AInputVATDifferencesTurnOutDebtorAC : IAccountingSubject
    {
        protected FKTZSServiceEntity context;
        public List<AccVouch> Load(FKTZSServiceEntity fktzsServiceEntity)
        {
            context = fktzsServiceEntity;
            if (!Verification())
                return null;
            return DoLoad();
        }
        protected virtual Boolean Verification()
        {
            //E单据肯定有此明细
            if (context.Fktzs_C_HEntitys.ZydType.ToUpper() == "E")
                return true;
            //2018-6-28 17:33:48逻辑废弃
            //实际支付金额合计 -暂估金额合计<>0时有此明细
            //if (context.Fktzs_C_HEntitys.SjjeTotal - context.Fktzs_C_HEntitys.ZgjeTotal != 0)
            //2018-6-28 17:35:20根据凭证货币金额不为0判断是否次明细
            if (GetWRBTR() != 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 根据实际支付金额/(1+税率[普票税率0])-开票时的不含税外包成本的合计金额(如果是E单就为0)判断：> 0,则40 < 0，则50
        /// </summary>
        /// <returns></returns>
        protected virtual string GetNEWBS()
        {
            if (GetWRBTR() > 0)
                return "40";
            else
                return "50";
        }
        /// <summary>
        /// 根据实际支付金额/(1+税率[普票税率0])-开票时的不含税外包成本的合计金额(如果是E单就为0)
        /// </summary>
        /// <returns></returns>
        protected virtual decimal GetWRBTR()
        {
            decimal wbcsTotal = context.Fktzs_C_HEntitys.ZydType.ToUpper() == "E" ? 0 : context.KpsqEntitys.WBCS_TotalAmount;
            decimal resultAmount = context.Fktzs_C_HEntitys.SjjeTotal / (1 + context.InvoiceEntitys.TaxTate) - wbcsTotal;
            return decimal.Round(resultAmount, 2);
        }
        protected abstract List<AccVouch> DoLoad();
    }
}
