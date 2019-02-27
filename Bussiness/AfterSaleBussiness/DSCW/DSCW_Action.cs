using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness.DSCW
{
    public class DSCW_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("DSCW执行开票申请数据凭证回传");
            KPSQ kpsq = new KPSQ("DSCW_Path_ASB".ToAppSetting(), "DSCW_KPSQ_Salse_Prefix".ToAppSetting(), "DSCW_KPSQ_Purchase_Prefix".ToAppSetting(), "DSCW_Ext_ASB".ToAppSetting(), "DSCW", "4040", this);
            kpsq.GetData();
            LogInfo.Log.Info("DSCW执行开票申请数据凭证回传成功");
            LogInfo.Log.Info("DSCW执行付款通知书（有偿）数据凭证回传");
            SAPLinks.Bussiness.AfterSaleBussiness.FKTZS fs = new SAPLinks.Bussiness.AfterSaleBussiness.FKTZS("DSCW", "4040", connStr, "DSCW06", "DSCW_FKTZSYC_Prefix".ToAppSetting(), "DSCW_Ext_ASB".ToAppSetting(), "DSCW_Path_ASB".ToAppSetting());
            fs.GetData();
            LogInfo.Log.Info("DSCW执行付款通知书（有偿）数据凭证回传成功");
        }
    }
}
