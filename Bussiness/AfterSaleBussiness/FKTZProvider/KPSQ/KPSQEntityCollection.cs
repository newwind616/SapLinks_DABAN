using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 开票申请明细实体集合
    /// </summary>
    public class KPSQEntityCollection:List<KPSQEntity>
    {
        public decimal WBCS_TotalAmount { get; set; }//开票不含税金额
        public static KPSQEntityCollection Load(ApplyNoEntity applyNoEntity)
        {
            return AggData(ExecuteQuery(applyNoEntity, StringFormat(applyNoEntity)));
        }
        private static string StringFormat(ApplyNoEntity applyNoEntity)
        {
            //AND SRC_TASKID<>CURRENT_TASKID为了过滤E单据
            string query = @"select BB.TASKID,BB.Z_GLBH,BB.C_WBCS from (
                                select * from BPMDB.DBO.MAIN_KPSQ_DATAUSED where CURRENT_TASKID={0} AND SRC_TASKID<>CURRENT_TASKID
                                ) AA
                                left join DABAN_BPM_{1}.DBO.KPSQ_ZYDXX_D BB on AA.SRC_TASKID=BB.TASKID and AA.XBLNR=BB.Z_GLBH AND AA.CBXZ=BB.Z_GLBH_PK";
            query = string.Format(query, applyNoEntity.TaskID, applyNoEntity.BasicEntity.Company);
            return query;
        }
        private static DataTable ExecuteQuery(ApplyNoEntity applyNoEntity, string query)
        {
            return SQLHelper.ExecuteDataset(applyNoEntity.BasicEntity.ConnStr, CommandType.Text, query).Tables[0];
        }
        private static KPSQEntityCollection AggData(DataTable dt)
        {
            KPSQEntityCollection invoices = new KPSQEntityCollection();
            foreach (DataRow item in dt.Rows)
            {
                KPSQEntity kPSQEntity = new KPSQEntity();
                kPSQEntity.XBLNR = Convert.ToString(item["Z_GLBH"]);
                kPSQEntity.WBCS = Convert.ToDecimal(item["C_WBCS"]);
                invoices.WBCS_TotalAmount += kPSQEntity.WBCS;
                invoices.Add(kPSQEntity);
            }
            return invoices;
        }
    }
}
