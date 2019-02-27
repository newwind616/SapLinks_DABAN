using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public interface IAccountingSubject
    {
        List<AccVouch> Load(FKTZSServiceEntity fktzsServiceEmpty);
    }
}
