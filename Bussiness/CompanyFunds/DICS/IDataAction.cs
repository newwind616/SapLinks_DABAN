using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DICS
{
    public interface IDataAction
    {
        void GetData(DICS_Action dics_Action);
    }
}
