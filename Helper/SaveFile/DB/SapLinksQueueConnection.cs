using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Helper.SaveFile
{
    public class SapLinksQueueConnection : LinksQueueConnection
    {
        public SapLinksQueueConnection(SapLinksQueueEntity sapLinksQueue) : base(sapLinksQueue)
        {
        }

        public override void Open()
        {
            cmd = SQLHelper.GetTransactionSqlCommand(this.sapLinksQueue.ConnStr);
        }
        public override void ExecuteQuery()
        {
            if (!string.IsNullOrEmpty(sapLinksQueue.UpdateSql))
                SQLHelper.ExecuteNonQuery(ref cmd, sapLinksQueue.UpdateSql);
        }

        public override void Commit()
        {
            if (!string.IsNullOrEmpty(sapLinksQueue.UpdateSql))
                cmd.Transaction.Commit();
        }

        public override void Rollback()
        {
            if (!string.IsNullOrEmpty(sapLinksQueue.UpdateSql))
                cmd.Transaction.Rollback();
        }
    }
}
