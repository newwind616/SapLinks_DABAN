using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness.DSC_B
{
    public class DSC_B_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("DSC_B执行开票申请数据凭证回传");
            KPSQ kpsq = new KPSQ("DSC_B_Path_ASB".ToAppSetting(), "DSC_B_KPSQ_Salse_Prefix".ToAppSetting(), "DSC_B_KPSQ_Purchase_Prefix".ToAppSetting(), "DSC_B_Ext_ASB".ToAppSetting(), "DSC_B", "4011", this);
            kpsq.GetData();
            LogInfo.Log.Info("DSC_B执行开票申请数据凭证回传成功");
            LogInfo.Log.Info("DSC_B执行付款通知书（有偿）数据凭证回传");
            SAPLinks.Bussiness.AfterSaleBussiness.FKTZS fs = new SAPLinks.Bussiness.AfterSaleBussiness.FKTZS("DSC_B", "4011", connStr, "DSC_B06", "DSC_B_FKTZSYC_Prefix".ToAppSetting(), "DSC_B_Ext_ASB".ToAppSetting(), "DSC_B_Path_ASB".ToAppSetting());
            fs.GetData();
            LogInfo.Log.Info("DSC_B执行付款通知书（有偿）数据凭证回传成功");
        }
    }
}
