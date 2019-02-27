using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public class FKTZSZYDEntityCollection:List<FKTZSZYDEntity>
    {
        public static FKTZSZYDEntityCollection Load(ApplyNoEntity applyNoEntity)
        {
            return AggData(ExecuteQuery(applyNoEntity, StringFormat(applyNoEntity)));
        }
        private static string StringFormat(ApplyNoEntity applyNoEntity)
        {
            string query = @"SELECT * FROM (
SELECT ID,TASKID,Z_GLBH,Z_GLBH_PK,Z_YWLX,Z_HTBH,Z_YYDD,KHBM,S_KPJE,C_SAKNR,C_SNWBMC,C_WBJE,SJZFJE FROM [DABAN_BPM_{1}].[DBO].[FKTZS_ZYD_D] WHERE TASKID={0}
UNION ALL 
SELECT ID,TASKID,Z_GLBH,Z_GLBH_PK,Z_YWLX,Z_HTBH,Z_YYDD,KHBM,S_KPJE,C_SAKNR,C_SNWBMC,C_WBJE,SJZFJE FROM [DABAN_BPM_{1}].[DBO].[FKTZS_ZYD_EXCEL_D] WHERE TASKID={0}
) AA ORDER BY ID ASC";
            query = string.Format(query, applyNoEntity.TaskID, applyNoEntity.BasicEntity.Company);
            return query;
        }
        private static DataTable ExecuteQuery(ApplyNoEntity applyNoEntity, string query)
        {
            return SQLHelper.ExecuteDataset(applyNoEntity.BasicEntity.ConnStr, CommandType.Text, query).Tables[0];
        }
        private static FKTZSZYDEntityCollection AggData(DataTable dt)
        {
            FKTZSZYDEntityCollection fKTZSZYDEntitys = new FKTZSZYDEntityCollection();
            foreach (DataRow item in dt.Rows)
            {
                FKTZSZYDEntity fKTZSZYDEntity = new FKTZSZYDEntity();
                fKTZSZYDEntity.ID = Convert.ToInt32(item["ID"]);
                fKTZSZYDEntity.TASKID = Convert.ToInt32(item["TASKID"]);
                fKTZSZYDEntity.Z_GLBH = Convert.ToString(item["Z_GLBH"]);
                fKTZSZYDEntity.Z_GLBH_PK = Convert.ToString(item["Z_GLBH_PK"]);
                fKTZSZYDEntity.Z_YWLX = Convert.ToString(item["Z_YWLX"]);
                if (fKTZSZYDEntity.Z_YWLX == "-")
                    fKTZSZYDEntity.Z_YWLX = "";
                fKTZSZYDEntity.Z_HTBH = Convert.ToString(item["Z_HTBH"]);
                fKTZSZYDEntity.Z_YYDD = Convert.ToString(item["Z_YYDD"]);
                fKTZSZYDEntity.KHBM = Convert.ToString(item["KHBM"]);
                fKTZSZYDEntity.S_KPJE = Convert.ToDecimal(item["S_KPJE"]);
                fKTZSZYDEntity.C_SAKNR = Convert.ToString(item["C_SAKNR"]);
                fKTZSZYDEntity.C_SNWBMC = Convert.ToString(item["C_SNWBMC"]);
                fKTZSZYDEntity.C_WBJE = Convert.ToString(item["C_WBJE"]) == "" ? 0 : Convert.ToDecimal(item["C_WBJE"]);
                fKTZSZYDEntity.SJZFJE = Convert.ToDecimal(item["SJZFJE"]);
                fKTZSZYDEntitys.Add(fKTZSZYDEntity);
            }
            return fKTZSZYDEntitys;
        }
    }
}
