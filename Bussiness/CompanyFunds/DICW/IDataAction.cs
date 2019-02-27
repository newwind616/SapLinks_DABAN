using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DICW
{
    public interface IDataAction
    {
        void GetData(DICW_Action dics_Action);
    }
}
