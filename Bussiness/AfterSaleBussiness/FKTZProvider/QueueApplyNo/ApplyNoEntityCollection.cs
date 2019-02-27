using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 单号实体集合
    /// </summary>
    public class ApplyNoEntityCollection:List<ApplyNoEntity>
    {
        public static ApplyNoEntityCollection Load(ApplyNoBasicEntity applyNoBasicEntity)
        {
            //ApplyNoBasicEntity applyNoBasicEntity = new ApplyNoBasicEntity();
            //applyNoBasicEntity.ApplyNoPrefix = "DSC04";
            //applyNoBasicEntity.Company = "DSC";
            //applyNoBasicEntity.ConnStr = ConnStr;
            //applyNoBasicEntity.LinksType = new LinkType(LinkType.LinksFalse);
            ISapLinksQueue iQueue = SapLinksQueueFactory.Init();
            ApplyNoEntityCollection applyNos = iQueue.Load(applyNoBasicEntity);
            return applyNos;
        }
    }
}
