using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DSC
{
    public interface IDataAction
    {
        void GetData(DSC_Action dics_Action);
    }
}
