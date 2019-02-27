using SAPLinks.Bussiness;
using SAPLinks.Bussiness.SAPToBPMResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.TempAction
{
    public class TempIncomePurchaseUpdateAction : BaseAction, ICompanyAction
    {
        AIncomePurchaseUpdate aIncomeUpdate;
        AIncomePurchaseUpdate aPurchaseUpdate;
        public TempIncomePurchaseUpdateAction() { }

        public void Start()
        {
            aIncomeUpdate = AIncomePurchaseUpdate.Create(connStr, new IncomePurchaseUpdateType(IncomePurchaseUpdateType.Income));
            aIncomeUpdate.Add("DSC").Add("DSCS").Add("DSCW");
            aIncomeUpdate.ExecuteQuery(true);
            aPurchaseUpdate = AIncomePurchaseUpdate.Create(connStr, new IncomePurchaseUpdateType(IncomePurchaseUpdateType.Purchase));
            aPurchaseUpdate.Add("DSC").Add("DSCS").Add("DSCW");
            aPurchaseUpdate.ExecuteQuery(true);
        }
    }
}
