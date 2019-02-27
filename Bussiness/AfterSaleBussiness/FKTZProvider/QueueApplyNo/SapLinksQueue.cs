using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// SAPLink队列数据获取
    /// </summary>
    public class SapLinksQueue : ISapLinksQueue
    {
        ApplyNoBasicEntity _applyNoBasicEntity;
        public SapLinksQueue()
        {
        }
        public ApplyNoEntityCollection Load(ApplyNoBasicEntity applyNoBasicEntity)
        {
            this._applyNoBasicEntity = applyNoBasicEntity;
            return AggData(ExecuteQuery(SqlFormat()));
        }
        /// <summary>
        /// 2018-8-21 16:00:40 取数逻辑由原有的_applyNoBasicEntity.ApplyNoPrefix改为_applyNoBasicEntity.Company + "06", _applyNoBasicEntity.Company + "08"
        /// </summary>
        /// <returns></returns>
        private string SqlFormat()
        {
            string query = @"select APPLY_NO,BB.TaskID,BB.FinishAt from BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_QUEUE AA
left join BPMDB.dbo.BPMInstTasks BB on AA.APPLY_NO=BB.SerialNum where(APPLY_NO like '{0}%' or APPLY_NO like '{1}%') and ISLINK={2} and COMPANY='{3}'";
            query = string.Format(query, ApplyNoConvert(_applyNoBasicEntity.Company + "06"), ApplyNoConvert(_applyNoBasicEntity.Company + "08"), _applyNoBasicEntity.LinksType.IsLinks, _applyNoBasicEntity.Company);
            return query;
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
        private DataTable ExecuteQuery(string query)
        {
            return SQLHelper.ExecuteDataset(_applyNoBasicEntity.ConnStr, CommandType.Text, query).Tables[0];
        }
        private ApplyNoEntityCollection AggData(DataTable dt)
        {
            ApplyNoEntityCollection applyNos = new ApplyNoEntityCollection();
            foreach (DataRow item in dt.Rows)
            {
                ApplyNoEntity applyNoEntity = new ApplyNoEntity();
                applyNoEntity.ApplyNo = Convert.ToString(item["APPLY_NO"]);
                applyNoEntity.TaskID = Convert.ToString(item["TaskID"]);
                applyNoEntity.FinishAt = Convert.ToDateTime(item["FinishAt"]).ToString("yyyyMMdd");
                applyNoEntity.BasicEntity = _applyNoBasicEntity;
                applyNos.Add(applyNoEntity);
            }
            return applyNos;
        }
    }
}
