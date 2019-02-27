using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DICN
{
    public interface IDataAction
    {
        void GetData(DICN_Action dicn_Action);
    }
}
