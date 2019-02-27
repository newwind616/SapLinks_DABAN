using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPAccount
{
    /// <summary>
    /// 凭证回传
    /// </summary>
    public class SAPToBPMResult_Action2 : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("执行SAP凭证数据回传解析");
            Go();
            LogInfo.Log.Info("执行SAP凭证数据回传解析成功");
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            SAPToBPMResultObject s_sap2 = new SAP2.SAP_COMPANYFUNDS_LINKS_FAILED("SAPToBPMResultQueue1".ToAppSetting(), "SAPToBPMResultSuccess1".ToAppSetting(), "SAPToBPMResultFaild1".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
