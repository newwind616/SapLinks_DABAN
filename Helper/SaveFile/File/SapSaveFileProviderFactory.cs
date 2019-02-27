using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Helper.SaveFile
{
    public class SapSaveFileProviderFactory
    {
        public static ISaveFileProvider CreateSaveProvider(SapLinksQueueEntity sapLinksQueue)
        {
            LinksQueueConnection linksQueueConnection = SapLinksQueueConnectionFactory.Create(sapLinksQueue);
            return new SapSaveFileProvider(linksQueueConnection);
        }
    }
}
