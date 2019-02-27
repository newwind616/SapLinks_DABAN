using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DICB
{
    public interface IDataAction
    {
        void GetData(DICB_Action dicb_Action);
    }
}
