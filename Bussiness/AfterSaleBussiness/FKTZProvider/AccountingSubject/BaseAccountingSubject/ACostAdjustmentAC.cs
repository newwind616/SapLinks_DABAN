using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 2：成本调整
    /// 生成成本-基本生产成本-外注费-有偿
    /// </summary>
    public abstract class ACostAdjustmentAC : IAccountingSubject
    {
        protected FKTZSServiceEntity context;
        public List<AccVouch> Load(FKTZSServiceEntity fktzsServiceEntity)
        {
            context = fktzsServiceEntity;
            if (!Verification())
                return null;
            return DoLoad();
        }
        protected virtual Boolean Verification()
        {
            return true;
        }
        protected abstract List<AccVouch> DoLoad();
    }
}
