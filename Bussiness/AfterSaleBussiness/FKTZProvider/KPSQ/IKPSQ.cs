using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public interface IKPSQ
    {
        KPSQEntityCollection Load(ApplyNoEntity applyNoEntity);
    }
}
