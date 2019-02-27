using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 基础必要信息
    /// </summary>
    public class ApplyNoBasicEntity
    {
        /// <summary>
        /// BPM连接字符串
        /// </summary>
        public string ConnStr { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 单号前缀
        /// </summary>
        public string ApplyNoPrefix { get; set; }
        /// <summary>
        /// 连携标志位
        /// </summary>
        public LinkType LinksType { get; set; }
        /// <summary>
        /// 付款通知书流程类型
        /// </summary>
        public FKTZSProcessType FktzsProcessType { get; set; }
        /// <summary>
        /// 付款通知书有偿无偿类型
        /// </summary>
        public FKTZSYCWCType FktzsYcWcType { get; set; }
    }
}
