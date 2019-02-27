using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.PersonalFunds.DCSL
{
    public class DCSLD : DataConvert
    {
        public DCSLD(CompanyObject action) : base(action)
        {
        }

        public override void GetData()
        {
            List<string> list = new List<string>();
            string sql = string.Format("exec DABAN_BPM_DCSL.DBO.P_GetSAPInfo @COMPANY ='{0}'", context.company);
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            LogInfo.Log.Info("《DCSL个人经费》获取需处理数量：" + dt.Rows.Count + "条");
            if (dt.Rows.Count == 0)
                return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Clear();
                //数据填充开始
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    list.Add(dt.Rows[i][j].ToString());
                }
                //数据填充结束
                file_sb.AppendLine(Create(list));
            }
            sql = string.Format(@"SELECT A.APPLY_NO
                    FROM BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_QUEUE A
                    INNER JOIN DABAN_BPM_{0}.DBO.FKTZS_P_H B
                    ON A.APPLY_NO = B.APPLY_NO
                    INNER JOIN BPMDB.DBO.MAIN_BATCHNO_H C
                    ON B.TASKID = C.TASKID
                    AND C.BATCHNO_TYPE = 'FinBatch'
                    WHERE A.COMPANY = '{0}'
                    AND A.ISLINK = 0", context.company);
            dt = SQLHelper.ExecuteDataset(context.connStr, CommandType.Text, sql).Tables[0];
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string applyNo = Convert.ToString(dt.Rows[i]["APPLY_NO"]);
                upLinks_sql.AppendLine(string.Format("UPDATE BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_QUEUE SET ISLINK = 1,SAP_DATE = '{0}' WHERE APPLY_NO = '{1}';", dateTime, applyNo));
            }
        }
    }
}
