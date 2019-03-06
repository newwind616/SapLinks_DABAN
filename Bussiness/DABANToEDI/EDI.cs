using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SAPLinks.Bussiness.DABANToEDI
{
    public class EDI : DataConvert
    {
        public EDI(EDIObject action) : base(action)
        {
        }
        public override void GetData()
        {
            List<string> list = new List<string>();
            List<string> idlist = new List<string>();
            string sql = string.Format("select ID, COMPANY, INV_CODE, INV_NO, INV_DATE, INV_NAME, INV_TIN,  AMOUNT, TAX,SAP_DIST from [BPMDB].[dbo].MAIN_EDI_DATA where STATE=0");
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            LogInfo.Log.Info("《DABANToEDI》获取需处理数量：" + dt.Rows.Count + "条");
            if (dt.Rows.Count == 0)
                return;
            file_sb.AppendLine("公司简称\t发票代码\t发票号码\t开票日期\t销方名称\t销方税号\t金额\t税额\tSAP供应商");
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

            //MAIN_EDI_DATA
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            upLinks_sql.AppendFormat(" update [BPMDB].[dbo].MAIN_EDI_DATA set STATE=1 where ID in({0}) ", string.Join(",", idlist.ToArray()));

        }
    }
}
