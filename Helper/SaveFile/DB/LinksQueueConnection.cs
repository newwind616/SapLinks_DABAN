using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SAPLinks.Helper.SaveFile
{
    public abstract class LinksQueueConnection : IDisposable
    {
        protected SapLinksQueueEntity sapLinksQueue { get; set; }
        public LinksQueueConnection(SapLinksQueueEntity sapLinksQueue)
        {
            this.sapLinksQueue = sapLinksQueue;
        }
        protected SqlCommand cmd;
        public abstract void Open();
        public abstract void ExecuteQuery();
        public abstract void Commit();
        public abstract void Rollback();
        public void Dispose()
        {
            if (!string.IsNullOrEmpty(sapLinksQueue.UpdateSql))
                cmd.Connection.Close();
        }
        public SapLinksQueueEntity GetSapLinksQueue()
        {
            return sapLinksQueue;
        }
    }
}
