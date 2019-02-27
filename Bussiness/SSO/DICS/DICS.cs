using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SAPLinks.Bussiness.SSO.DICS
{
    public class DICS : DataConvert
    {

        public DICS(CompanyObject action) : base(action)
        {

        }

        public override void GetData()
        {
            List<string> list = new List<string>();
            List<string> idlist = new List<string>();
            //string sql = string.Format("exec DABAN_BPM_DICS.DBO.P_GetGSSapInfo @COMPANY ='{0}'", context.company);
            string sql = @"select A.ID, B.APPLY_ACCOUNT,C.SYSTEM_ID,C.FLG from [BPMDB].[dbo].[SAP_COMPANYFUNDS_LINKS_QUEUE] A
                            INNER JOIN DABAN_BPM_DICS.dbo.SSO_USER_H B
                            ON A.APPLY_NO=B.APPLY_NO
                            INNER JOIN DABAN_BPM_DICS.dbo.SSO_USER_D C
                            ON B.TASKID=C.TASKID
                            WHERE A.ISLINK=0";
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            LogInfo.Log.Info("《DICS_SSO》获取需处理数量：" + dt.Rows.Count + "条");
            if (dt.Rows.Count == 0)
                return;
            file_sb.AppendLine("User Login Name,WebAppID,Status");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                idlist.Add(dt.Rows[i][0].ToString());

                list.Clear();
                //数据填充开始
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    list.Add(dt.Rows[i][j].ToString());
                }
                //数据填充结束
                file_sb.AppendLine(Create(list));
            }

            //更新SAP_COMPANYFUNDS_LINKS_QUEUE
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            upLinks_sql.AppendFormat(" update [BPMDB].[dbo].[SAP_COMPANYFUNDS_LINKS_QUEUE] set ISLINK=1,SAP_DATE = '{1}' where ID in({0}) ", string.Join(",", idlist.ToArray()), dateTime);
            
        }
    }

}
