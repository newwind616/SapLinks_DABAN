using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.BinYi_Synchor.DICW
{
    public class DICW_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("执行存储过程【DABAN_BPM_DICW.dbo.P_GetKFInfo】");
            SQLHelper.ExecuteScalar(connStr, System.Data.CommandType.Text, "exec DABAN_BPM_DICW.dbo.P_GetKFInfo");
            LogInfo.Log.Info("执行存储过程【DABAN_BPM_DICW.dbo.P_GetKFInfo】完成");
        }
    }
}
