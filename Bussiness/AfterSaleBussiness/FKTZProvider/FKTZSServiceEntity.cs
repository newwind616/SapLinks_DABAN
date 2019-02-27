using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 付款通知书数据提取总方法，将开票、发票信息等获取
    /// </summary>
    public class FKTZSServiceEntity
    {
        /// <summary>
        /// 基本信息
        /// </summary>
        public ApplyNoEntity ApplyNoEntity { get; set; }
        /// <summary>
        /// 付款通知书表头
        /// </summary>
        public FKTZS_C_HEntity Fktzs_C_HEntitys { get; set; }
        /// <summary>
        /// 发票明细
        /// </summary>
        public InvoiceEntityCollection InvoiceEntitys { get; set; }
        /// <summary>
        /// 开票申请明细
        /// </summary>
        public KPSQEntityCollection KpsqEntitys { get; set; }
        /// <summary>
        /// 开票申请明细
        /// </summary>
        public FKTZSZYDEntityCollection FKTZSZYDEntitys { get; set; }
        public static FKTZSServiceEntity Load(ApplyNoEntity applyNoEntity)
        {
            FKTZSServiceEntity fktzsServiceEntity = new FKTZSServiceEntity();
            fktzsServiceEntity.ApplyNoEntity = applyNoEntity;
            fktzsServiceEntity.Fktzs_C_HEntitys = FKTZS_C_HEntity.Load(applyNoEntity);
            fktzsServiceEntity.InvoiceEntitys = InvoiceEntityCollection.Load(applyNoEntity);
            //2018-8-21 11:57:44改造开票申请取数逻辑。开票申请有偿、无偿的取数表不同
            fktzsServiceEntity.KpsqEntitys = KPSQFactory.Create(applyNoEntity).Load(applyNoEntity);//KPSQEntityCollection.Load(applyNoEntity);
            fktzsServiceEntity.FKTZSZYDEntitys = FKTZSZYDEntityCollection.Load(applyNoEntity);
            return fktzsServiceEntity;
        }
    }
}
