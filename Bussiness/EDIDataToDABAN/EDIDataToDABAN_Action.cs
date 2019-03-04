using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.EDIDataToDABAN
{
    public class EDIDataToDABAN_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("执行主数据同步");
            DTCProvider dtc = new DTCProvider(Go);
            dtc.Start();
        }

        private void Go()
        {
            Center_Subject center = new Center_Subject();
            EDIDataToDABANObject ediob = new EDI.MAIN_EDI_DATA("MAIN_EDI".ToAppSetting(), this, center);
            center.Refresh();
        }
    }
}
