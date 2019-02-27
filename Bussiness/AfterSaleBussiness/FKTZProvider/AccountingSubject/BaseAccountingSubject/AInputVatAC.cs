using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public abstract class AInputVatAC : IAccountingSubject
    {
        protected FKTZSServiceEntity context;
        public List<AccVouch> Load(FKTZSServiceEntity fktzsServiceEntity)
        {
            this.context = fktzsServiceEntity;
            if (!Verification())
                return null;
            return DoLoad();
        }
        protected virtual Boolean Verification()
        {
            if (context.Fktzs_C_HEntitys.ZydType.ToUpper() == "E")
                return false;
            return true;
        }
        protected abstract List<AccVouch> DoLoad();
    }
}
