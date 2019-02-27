using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPIncome
{
    public class SAPIncome_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("执行收入");
            Go();
            LogInfo.Log.Info("执行收入成功");
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            SAPToBPMResultObject s_sap1 = new SAP1.SAPIncome("SAPIncomeQueue".ToAppSetting(), "SAPIncomeSuccess".ToAppSetting(), "SAPIncomeFaild".ToAppSetting(), this, center);
            //SAPToBPMResultObject s_sap2 = new SAP2.SAPIncome("SAPToBPMResultQueue1".ToAppSetting(), "SAPToBPMResultSuccess1".ToAppSetting(), "SAPToBPMResultFaild1".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
