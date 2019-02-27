using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness.DSC
{
    public class DSC_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("DSC执行开票申请数据凭证回传");
            KPSQ kpsq = new KPSQ("DSC_Path_ASB".ToAppSetting(), "DSC_KPSQ_Salse_Prefix".ToAppSetting(), "DSC_KPSQ_Purchase_Prefix".ToAppSetting(), "DSC_Ext_ASB".ToAppSetting(), "DSC", "4000", this);
            kpsq.GetData();
            LogInfo.Log.Info("DSC执行开票申请数据凭证回传成功");
            LogInfo.Log.Info("DSC执行付款通知书（有偿）数据凭证回传");
            SAPLinks.Bussiness.AfterSaleBussiness.FKTZS fs = new SAPLinks.Bussiness.AfterSaleBussiness.FKTZS("DSC", "4000", connStr, "DSC06", "DSC_FKTZSYC_Prefix".ToAppSetting(), "DSC_Ext_ASB".ToAppSetting(), "DSC_Path_ASB".ToAppSetting());
            fs.GetData();
            LogInfo.Log.Info("DSC执行付款通知书（有偿）数据凭证回传成功");
        }
    }
}
