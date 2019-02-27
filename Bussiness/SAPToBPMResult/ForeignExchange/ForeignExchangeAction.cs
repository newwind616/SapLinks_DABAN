using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.ForeignExchange
{
    /// <summary>
    /// 八爪鱼汇率回传
    /// </summary>
    public class ForeignExchangeAction : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("开始执行汇率回传");
            Go();
            LogInfo.Log.Info("执行汇率回传完毕");
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            SAPToBPMResultObject s_sap1 = new ForeignExchange("ForeignExchangeQueue".ToAppSetting(), "ForeignExchangeSuccess".ToAppSetting(), "ForeignExchangeFaild".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
