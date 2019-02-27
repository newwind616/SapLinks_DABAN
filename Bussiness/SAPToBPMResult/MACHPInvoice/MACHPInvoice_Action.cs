using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.MACHPInvoice
{
    public class MACHPInvoice : BaseAction, ICompanyAction
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
            SAPToBPMResultObject s_sap1 = new MAC1.MACHPPInvoice("MACHPInvoiceQueue".ToAppSetting(), "MACHPInvoiceSuccess".ToAppSetting(), "MACHPInvoiceFaild".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
