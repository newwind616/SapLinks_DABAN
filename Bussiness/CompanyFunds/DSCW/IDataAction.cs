using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DSCW
{
    public interface IDataAction
    {
        void GetData(DSCW_Action dics_Action);
    }
}
