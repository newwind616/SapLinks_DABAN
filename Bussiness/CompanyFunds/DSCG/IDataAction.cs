using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DSCG
{
    public interface IDataAction
    {
        void GetData(DSCG_Action dics_Action);
    }
}
