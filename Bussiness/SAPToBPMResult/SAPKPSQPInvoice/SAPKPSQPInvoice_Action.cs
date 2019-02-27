using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPKPSQPInvoice
{
    public class SAPKPSQPInvoice : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("开始执行开票回传");
            Go();
            LogInfo.Log.Info("执行开票回传完毕");
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            SAPToBPMResultObject s_sap1 = new SAP1.SAPKPSQPInvoice("KPSQPInvoiceQueue".ToAppSetting(), "KPSQPInvoiceSuccess".ToAppSetting(), "KPSQPInvoiceFaild".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
