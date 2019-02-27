using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 付款通知书可生成凭证方法绑定
    /// </summary>
    public class FKTZSServiceManager
    {
        public static List<AccVouch> Load(FKTZSServiceEntity fktzsServiceEntity, FKTZSServiceManagerEntity fktzsServiceManager)
        {
            //fktzsServiceManager = new FKTZSServiceManagerEntity();
            fktzsServiceManager.loadDataHander += fktzsServiceManager.iAccountingSubject_InputVatAC.Load;
            fktzsServiceManager.loadDataHander += fktzsServiceManager.iAccountingSubject_CostAdjustmentAC.Load;
            fktzsServiceManager.loadDataHander += fktzsServiceManager.iAccountingSubject_AccountPayableAdvanceReceivedAC.Load;
            fktzsServiceManager.loadDataHander += fktzsServiceManager.iAccountingSubject_InputVATDifferencesTurnOutDebtorAC.Load;
            fktzsServiceManager.loadDataHander += fktzsServiceManager.iAccountingSubject_ActualPayableAC.Load;
            fktzsServiceManager.loadDataHander += fktzsServiceManager.iAccountingSubject_InputVATDifferenceAdjustmentAC.Load;
            fktzsServiceManager.loadDataHander += fktzsServiceManager.iAccountingSubject_InputVATDifferencesTurnOutCreditAC.Load;
            return fktzsServiceManager.Run(fktzsServiceEntity);
        }
    }
}
