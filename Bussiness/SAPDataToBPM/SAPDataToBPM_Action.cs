using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPDataToBPM
{
    public class SAPDataToBPM_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("执行主数据同步");
            DTCProvider dtc = new DTCProvider(Go);
            dtc.Start();
            //Go();
        }
        private void Go()
        {
            Center_Subject center = new Center_Subject();
            SAPDataToBPMObject s_sap1 = new SAP1.MAIN_SUPPLIER("MAIN_SUPPLIER".ToAppSetting(), "MAIN_SUPPLIER_Filter".ToAppSetting(),this, center);
            SAPDataToBPMObject s_sap2 = new SAP2.MAIN_SUPPLIER("MAIN_SUPPLIER1".ToAppSetting(), "MAIN_SUPPLIER_Filter1".ToAppSetting(), this, center);
            SAPDataToBPMObject c_sap1 = new SAP1.MAIN_COSTCENTER("MAIN_COSTCENTER".ToAppSetting(), "MAIN_COSTCENTER_Filter".ToAppSetting(), this, center);
            SAPDataToBPMObject c_sap2 = new SAP2.MAIN_COSTCENTER("MAIN_COSTCENTER1".ToAppSetting(), "MAIN_COSTCENTER_Filter1".ToAppSetting(), this, center);
            SAPDataToBPMObject cst_sap1 = new SAP1.MAIN_CUSTOMER("MAIN_CUSTOMER".ToAppSetting(), "MAIN_CUSTOMER_Filter".ToAppSetting(), this, center);
            //SAPDataToBPMObject cst_sap2 = new SAP2.MAIN_CUSTOMER("MAIN_CUSTOMER1".ToAppSetting(), "MAIN_CUSTOMER_Filter1".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
