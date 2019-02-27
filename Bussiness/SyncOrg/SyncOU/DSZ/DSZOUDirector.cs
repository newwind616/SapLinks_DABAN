using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SyncOrg.SyncOU.DSZ
{
    public class DSZOUDirector
    {
        OUBuilder ouHandler;
        public DSZOUDirector(OUBuilder ouHandler)
        {
            this.ouHandler = ouHandler;
        }
        /// <summary>
        /// 分步骤处理操作组织架构
        /// </summary>
        public virtual void Construct()
        {
            ouHandler.DeleteOU();
            ouHandler.NewOU();
            ouHandler.OUNoMoveNo();
            ouHandler.OUNoMoveYes();
            ouHandler.OUYesMoveNo();
            ouHandler.OUYesMoveYes();
            ouHandler.Exception();
        }
    }
}
