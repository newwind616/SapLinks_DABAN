using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPPurchase
{
    public class SAPPurchase_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("执行采购");
            Go();
            LogInfo.Log.Info("执行采购成功");
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            //SAPToBPMResultObject s_sap1 = new SAP1.SAPPurchase("SAPPurchaseQueue".ToAppSetting(), "SAPPurchaseSuccess".ToAppSetting(), "SAPPurchaseFaild".ToAppSetting(), this, center);
            //SAPToBPMResultObject s_sap1HD = new SAP1.SAPPurchaseHD("SAPPurchaseQueue".ToAppSetting(), "SAPPurchaseSuccess".ToAppSetting(), "SAPPurchaseFaild".ToAppSetting(), this, center);
            SAPToBPMResultObject s_sap1New = new SAP1.SAPPurchaseNew("SAPPurchaseQueue".ToAppSetting(), "SAPPurchaseSuccess".ToAppSetting(), "SAPPurchaseFaild".ToAppSetting(), this, center);
            //SAPToBPMResultObject s_sap2 = new SAP2.SAPPurchase("SAPToBPMResultQueue1".ToAppSetting(), "SAPToBPMResultSuccess1".ToAppSetting(), "SAPToBPMResultFaild1".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
