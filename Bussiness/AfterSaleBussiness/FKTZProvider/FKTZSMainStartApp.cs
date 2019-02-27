using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public class FKTZSMainStartApp : FKTZSMainBaseStartApp
    {
        /// <summary>
        /// 凭证科目实例化
        /// 2018-8-21 13:32:10改造，
        /// 根据单号区分有偿、无偿取数格式
        /// </summary>
        /// <returns></returns>
        protected override FKTZSServiceManagerEntity InitFKTZSServiceManagerEntity(ApplyNoEntity applyNoEntity)
        {
            FKTZSServiceManagerEntity fktzsServiceManagerEntity = new FKTZSServiceManagerEntity();
            //DICS06营业有偿
            if (applyNoEntity.BasicEntity.FktzsYcWcType.YCWCType == FKTZSYCWCType.YC)
            {
                fktzsServiceManagerEntity.SetAccountPayableAdvanceReceivedAC().SetActualPayableAC().SetCostAdjustmentAC("SAPLinks.Bussiness.AfterSaleBussiness.YC.CostAdjustmentAC").SetInputVatAC().
                SetInputVATDifferenceAdjustmentAC().SetInputVATDifferencesTurnOutCreditAC().SetInputVATDifferencesTurnOutDebtorAC();
            }
            else
            {
                fktzsServiceManagerEntity.SetAccountPayableAdvanceReceivedAC("SAPLinks.Bussiness.AfterSaleBussiness.WC.AccountPayableAdvanceReceivedAC").
                    SetActualPayableAC("SAPLinks.Bussiness.AfterSaleBussiness.WC.ActualPayableAC").SetCostAdjustmentAC("SAPLinks.Bussiness.AfterSaleBussiness.WC.CostAdjustmentAC").
                    SetInputVatAC("SAPLinks.Bussiness.AfterSaleBussiness.WC.InputVatAC").SetInputVATDifferenceAdjustmentAC("SAPLinks.Bussiness.AfterSaleBussiness.WC.InputVATDifferenceAdjustmentAC").
                    SetInputVATDifferencesTurnOutCreditAC("SAPLinks.Bussiness.AfterSaleBussiness.WC.InputVATDifferencesTurnOutCreditAC").SetInputVATDifferencesTurnOutDebtorAC("SAPLinks.Bussiness.AfterSaleBussiness.WC.InputVATDifferencesTurnOutDebtorAC");
            }
            return fktzsServiceManagerEntity;//SAPLinks.Bussiness.AfterSaleBussiness
        }
    }
}
