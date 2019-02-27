using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public class KPSQFactory
    {
        public static IKPSQ Create(ApplyNoEntity applyNoEntity)
        {
            if (applyNoEntity.BasicEntity.FktzsYcWcType.YCWCType==FKTZSYCWCType.YC)
            {
                return new KPSQ_YC();
            }
            else
            {
                return new KPSQ_WC();
            }
        }
    }
}
