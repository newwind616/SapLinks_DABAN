using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// SAP_COMPANYFUNDS_LINKS_QUEUE是否连携
    /// </summary>
    public class LinkType
    {
        /// <summary>
        /// 未连携
        /// </summary>
        public const string LinksFalse = "0";
        /// <summary>
        /// 已连携
        /// </summary>
        //public const string LinksTrue = "1";
        public string IsLinks { get; set; }
        public LinkType(string linkType)
        {
            this.IsLinks = linkType;
        }
    }
}
