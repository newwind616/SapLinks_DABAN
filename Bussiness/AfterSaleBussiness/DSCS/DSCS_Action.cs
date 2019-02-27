using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness.DSCS
{
    public class DSCS_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("DSCS执行开票申请数据凭证回传");
            KPSQ kpsq = new KPSQ("DSCS_Path_ASB".ToAppSetting(), "DSCS_KPSQ_Salse_Prefix".ToAppSetting(), "DSCS_KPSQ_Purchase_Prefix".ToAppSetting(), "DSCS_Ext_ASB".ToAppSetting(), "DSCS", "4020", this);
            kpsq.GetData();
            LogInfo.Log.Info("DSCS执行开票申请数据凭证回传成功");
            LogInfo.Log.Info("DSCS执行付款通知书（有偿）数据凭证回传");
            SAPLinks.Bussiness.AfterSaleBussiness.FKTZS fs = new SAPLinks.Bussiness.AfterSaleBussiness.FKTZS("DSCS", "4020", connStr, "DSCS06", "DSCS_FKTZSYC_Prefix".ToAppSetting(), "DSCS_Ext_ASB".ToAppSetting(), "DSCS_Path_ASB".ToAppSetting());
            fs.GetData();
            LogInfo.Log.Info("DSCS执行付款通知书（有偿）数据凭证回传成功");
        }
    }
}
