using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public abstract class AAccountPayableAdvanceReceivedAC : IAccountingSubject
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
            if (context.InvoiceEntitys.InvoiceType.IsSpecialInvoice)
                return true;
            else
                return false;
        }
        protected abstract List<AccVouch> DoLoad();
    }
}
