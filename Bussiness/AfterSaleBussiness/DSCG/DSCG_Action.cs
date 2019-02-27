using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness.DSCG
{
    public class DSCG_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("DSCG执行开票申请数据凭证回传");
            KPSQ kpsq = new KPSQ("DSCG_Path_ASB".ToAppSetting(), "DSCG_KPSQ_Salse_Prefix".ToAppSetting(), "DSCG_KPSQ_Purchase_Prefix".ToAppSetting(), "DSCG_Ext_ASB".ToAppSetting(), "DSCG", "4030", this);
            kpsq.GetData();
            LogInfo.Log.Info("DSCG执行开票申请数据凭证回传成功");
            LogInfo.Log.Info("DSCG执行付款通知书（有偿）数据凭证回传");
            SAPLinks.Bussiness.AfterSaleBussiness.FKTZS fs = new SAPLinks.Bussiness.AfterSaleBussiness.FKTZS("DSCG", "4030", connStr, "DSCG06", "DSCG_FKTZSYC_Prefix".ToAppSetting(), "DSCG_Ext_ASB".ToAppSetting(), "DSCG_Path_ASB".ToAppSetting());
            fs.GetData();
            LogInfo.Log.Info("DSCG执行付款通知书（有偿）数据凭证回传成功");
        }
    }
}
