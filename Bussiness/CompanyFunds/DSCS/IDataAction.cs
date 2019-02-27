using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DSCS
{
    public interface IDataAction
    {
        void GetData(DSCS_Action dics_Action);
    }
}
