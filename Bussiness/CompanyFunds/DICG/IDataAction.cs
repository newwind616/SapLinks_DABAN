using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DICG
{
    public interface IDataAction
    {
        void GetData(DICG_Action dics_Action);
    }
}
