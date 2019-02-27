using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SECTION
{
    public class SECTION_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("SECTION执行收入");
            Go();
            LogInfo.Log.Info("SECTION执行收入结束");
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            SAPToBPMResultObject s_sap1 = new SECTION("SECTIONQueue".ToAppSetting(), "SECTIONSuccess".ToAppSetting(), "SECTIONFaild".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
