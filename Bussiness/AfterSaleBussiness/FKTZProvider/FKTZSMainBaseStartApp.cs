using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 付款通知书主入口
    /// </summary>
    public class FKTZSMainBaseStartApp : IFKTZSMainStartApp
    {
        ApplyNoEntityCollection applyNos;
        public List<AccVouch> Load(ApplyNoBasicEntity applyNoBasicEntity)
        {
            List<AccVouch> listAccVouch = new List<AccVouch>();
            ISapLinksQueue iQueue = SapLinksQueueFactory.Init();
            applyNos = iQueue.Load(applyNoBasicEntity);
            foreach (ApplyNoEntity item in applyNos)
            {
                //拦截：判断是有偿单据还是无偿单据
                //if (item.BasicEntity.Company + "06" == item.ApplyNo.Substring(0, item.BasicEntity.Company.Length + 2))
                if(item.ApplyNo.StartsWith(ApplyNoConvert(item.BasicEntity.Company + "06")))
                {
                    item.BasicEntity.ApplyNoPrefix = ApplyNoConvert(item.BasicEntity.Company) + "06";
                    item.BasicEntity.FktzsProcessType = new FKTZSProcessType(FKTZSProcessType.FKTZS_YC);
                    item.BasicEntity.FktzsYcWcType = new FKTZSYCWCType(FKTZSYCWCType.YC);
                }
                //if (item.BasicEntity.Company + "08" == item.ApplyNo.Substring(0, item.BasicEntity.Company.Length + 2))
                if (item.ApplyNo.StartsWith(ApplyNoConvert(item.BasicEntity.Company + "08")))
                {
                    item.BasicEntity.ApplyNoPrefix = ApplyNoConvert(item.BasicEntity.Company) + "08";
                    item.BasicEntity.FktzsProcessType = new FKTZSProcessType(FKTZSProcessType.FKTZS_WC);
                    item.BasicEntity.FktzsYcWcType = new FKTZSYCWCType(FKTZSYCWCType.WC);
                }
                FKTZSServiceEntity fktzsServiceEntity = FKTZSServiceEntity.Load(item);
                List<AccVouch> list = FKTZSServiceManager.Load(fktzsServiceEntity, InitFKTZSServiceManagerEntity(item));
                list.MergeListAccVouch(listAccVouch);
            }
            return listAccVouch;
        }
        /// <summary>
        /// DSC_B06
        /// 2018-11-1 09:26:49申请编码去除下划线
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ApplyNoConvert(string str)
        {
            return str.Replace("_", "");
        }
        public ApplyNoEntityCollection GetApplyNoEntitys()
        {
            return applyNos;
        }
        /// <summary>
        /// 凭证科目实例化(此方法已被重写)
        /// </summary>
        /// <returns></returns>
        protected virtual FKTZSServiceManagerEntity InitFKTZSServiceManagerEntity(ApplyNoEntity item)
        {
            FKTZSServiceManagerEntity fktzsServiceManagerEntity = new FKTZSServiceManagerEntity();
            fktzsServiceManagerEntity.SetAccountPayableAdvanceReceivedAC().SetActualPayableAC().SetCostAdjustmentAC("SAPLinks.Bussiness.AfterSaleBussiness.CostAdjustmentAC").SetInputVatAC().
            SetInputVATDifferenceAdjustmentAC().SetInputVATDifferencesTurnOutCreditAC().SetInputVATDifferencesTurnOutDebtorAC();
            return fktzsServiceManagerEntity;//SAPLinks.Bussiness.AfterSaleBussiness
        }
    }
}
