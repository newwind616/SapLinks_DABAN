using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public class SapLinksQueueFactory
    {
        public static ISapLinksQueue Init()
        {
            ISapLinksQueue iSapLinkQueue = new SapLinksQueue();
            return iSapLinkQueue;
        }
    }
}
