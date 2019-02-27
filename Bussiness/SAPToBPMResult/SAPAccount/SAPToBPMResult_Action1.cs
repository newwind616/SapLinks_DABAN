using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPAccount
{
    /// <summary>
    /// 凭证回传
    /// </summary>
    public class SAPToBPMResult_Action1 : BaseAction, ICompanyAction
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
            SAPToBPMResultObject s_sap1 = new SAP1.SAP_COMPANYFUNDS_LINKS_FAILED("SAPToBPMResultQueue".ToAppSetting(), "SAPToBPMResultSuccess".ToAppSetting(), "SAPToBPMResultFaild".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
