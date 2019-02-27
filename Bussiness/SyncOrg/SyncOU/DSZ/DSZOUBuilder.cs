using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SyncOrg.SyncOU.DSZ
{
    public class DSZOUBuilder:OUBuilder
    {
        public DSZOUBuilder(DataTable oudata, BaseAction baseAction) : base(oudata, baseAction)
        {
        }

        public override void DeleteOU()
        {
            throw new NotImplementedException();
        }

        public override void NewOU()
        {
            throw new NotImplementedException();
        }

        public override void OUNoMoveNo()
        {
            throw new NotImplementedException();
        }

        public override void OUNoMoveYes()
        {
            throw new NotImplementedException();
        }

        public override void OUYesMoveNo()
        {
            throw new NotImplementedException();
        }

        public override void OUYesMoveYes()
        {
            throw new NotImplementedException();
        }
    }
}
