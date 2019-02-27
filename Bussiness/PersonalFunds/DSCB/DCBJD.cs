using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.PersonalFunds.DCBJ
{
    public class DCBJD : DataConvert
    {
        public DCBJD(CompanyObject action) : base(action)
        {
        }

        public override void GetData()
        {
            List<string> list = new List<string>();
            string sql = string.Format("exec DABAN_BPM_DCBJ.DBO.P_GetSAPInfo @COMPANY ='{0}'", context.company);
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            LogInfo.Log.Info("《DCBJ个人经费》获取需处理数量：" + dt.Rows.Count + "条");
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
            upLinks_sql.AppendLine(string.Format("SELECT * FROM BPMSYSUSERS where Account=''", context.company));
        }
    }
}
