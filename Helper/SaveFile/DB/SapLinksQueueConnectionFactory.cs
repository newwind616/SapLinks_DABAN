using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Helper.SaveFile
{
    public class SapLinksQueueConnectionFactory
    {
        public static LinksQueueConnection Create(SapLinksQueueEntity sapLinksQueue)
        {
            LinksQueueConnection sapLinksQueueConnection = new SapLinksQueueConnection(sapLinksQueue);
            return sapLinksQueueConnection;
        }
    }
}
