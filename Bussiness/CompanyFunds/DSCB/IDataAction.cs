using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DSCB
{
    public interface IDataAction
    {
        void GetData(DSCB_Action dics_Action);
    }
}
