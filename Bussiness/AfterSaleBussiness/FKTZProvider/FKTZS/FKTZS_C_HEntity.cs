using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 付款通知书表头主信息提取
    /// </summary>
    public class FKTZS_C_HEntity
    {
        public string ApplyDisplayName { get; set; }
        public string ApplyDept { get; set; }
        public string ApplyDeptCode { get; set; }
        public string PayObjId { get; set; }
        public string PayObjName { get; set; }
        /// <summary>
        /// 作业单类型E、C、G
        /// </summary>
        public string ZydType { get; set; }
        /// <summary>
        /// 暂估金额
        /// </summary>
        public decimal ZgjeTotal{get;set;}
        /// <summary>
        /// 实际金额
        /// </summary>
        public decimal SjjeTotal{get;set;}
        public static FKTZS_C_HEntity Load(ApplyNoEntity applyNoEntity)
        {
            return AggData(ExecuteQuery(applyNoEntity, StringFormat(applyNoEntity)));
        }
        private static string StringFormat(ApplyNoEntity applyNoEntity)
        {
            string query = @"select TASKID,APPLY_NO,APPLY_DISPLAYNAME,APPLY_DEPT,APPLY_DEPTCODE,PAY_OBJ_ID,PAY_OBJ_NAME,XREF2_HD,WBJE_TOTAL,WBCS_TOTAL 
                            from DABAN_BPM_{0}.DBO.FKTZS_C_H where PROCESS_TYPE = '{1}' and YCWC_TYPE = '{2}' and APPLY_NO='{3}'";
            query = string.Format(query, applyNoEntity.BasicEntity.Company, applyNoEntity.BasicEntity.FktzsProcessType.ProcessType, applyNoEntity.BasicEntity.FktzsYcWcType.YCWCType,applyNoEntity.ApplyNo);
            return query;
        }
        private static DataTable ExecuteQuery(ApplyNoEntity applyNoEntity, string query)
        {
            return SQLHelper.ExecuteDataset(applyNoEntity.BasicEntity.ConnStr, CommandType.Text, query).Tables[0];
        }
        private static FKTZS_C_HEntity AggData(DataTable dt)
        {
            FKTZS_C_HEntity fktzs_C_HEntity = new FKTZS_C_HEntity();
            foreach (DataRow item in dt.Rows)
            {
                fktzs_C_HEntity.ApplyDisplayName = Convert.ToString(item["APPLY_DISPLAYNAME"]);
                fktzs_C_HEntity.ApplyDept = Convert.ToString(item["APPLY_DEPT"]);
                fktzs_C_HEntity.ApplyDeptCode= Convert.ToString(item["APPLY_DEPTCODE"]);
                fktzs_C_HEntity.PayObjId = Convert.ToString(item["PAY_OBJ_ID"]);
                fktzs_C_HEntity.PayObjName = Convert.ToString(item["PAY_OBJ_NAME"]);
                fktzs_C_HEntity.ZydType = Convert.ToString(item["XREF2_HD"]);
                fktzs_C_HEntity.ZgjeTotal = Convert.ToDecimal(item["WBJE_TOTAL"]);//暂估金额合计
                fktzs_C_HEntity.SjjeTotal = Convert.ToDecimal(item["WBCS_TOTAL"]);//实际金额合计
            }
            return fktzs_C_HEntity;
        }
    }
}
