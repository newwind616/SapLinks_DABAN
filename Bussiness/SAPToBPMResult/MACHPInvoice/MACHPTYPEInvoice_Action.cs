using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.MACHPTYPEInvoice
{
    public class MACHPTYPEInvoice : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("开始执行供应商红票状态回传");
            Go();
            LogInfo.Log.Info("执行供应商红票状态回传完毕");
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            SAPToBPMResultObject s_sap1 = new MAC2.MACHPTYPEInvoice("MACHPTYPEInvoiceQueue".ToAppSetting(), "MACHPTYPEInvoiceSuccess".ToAppSetting(), "MACHPTYPEInvoiceFaild".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
