using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPPay
{
    public class SAPPAYHPInvoice : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("开始执行供应商红票SAPPAY回传");
            Go();
            LogInfo.Log.Info("执行供应商红票SAPPAY回传完毕");
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            SAPToBPMResultObject s_sap1 = new SAP1.SAP_PAY("SAPPAYHPInvoiceQueue".ToAppSetting(), "SAPPAYHPInvoiceSuccess".ToAppSetting(), "SAPPAYHPInvoiceFaild".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
