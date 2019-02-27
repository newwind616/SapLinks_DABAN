using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPDataToBPM
{
    public class SAPDataToBPM_ACCVOUCH_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("执行凭证数据回传接口");
            Go();
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            SAPDataToBPMObject s_sap1 = new SAP1.MAIN_SAP_FIN_ACCVOUCH("MAIN_SAP_FIN_ACCVOUCH1".ToAppSetting(), null, this, center);
            //SAPDataToBPMObject s_sap2 = new SAP2.MAIN_SAP_FIN_ACCVOUCH("MAIN_SAP_FIN_ACCVOUCH2".ToAppSetting(), null, this, center);
            center.Refresh();
        }
    }
}
