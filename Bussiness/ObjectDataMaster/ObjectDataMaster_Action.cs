using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.ObjectDataMaster
{
    public class ObjectDataMaster_Action: BaseAction, ICompanyAction
    {
        public ObjectDataMaster_Action() { }

        public void Start()
        {
            string sql = string.Format("exec DABAN_BPM_DICS.DBO.proc_UpdateObjectDataMaster");
            SQLHelper.ExecuteDataset(connStr, System.Data.CommandType.Text, sql);
        }
    }
}
