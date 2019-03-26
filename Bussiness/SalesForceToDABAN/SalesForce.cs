using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SAPLinks.Bussiness.SalesForceToDABAN
{
    public class SalesForce : DataConvert
    {
        public SalesForce(SalesForceObject action) : base(action)
        {
        }

        public override void GetData()
        {
            List<string> list = new List<string>();
            string sql = string.Format("select Name__c,Email__c,DICName__c,Area__c,BusinessType__c,Title__c,CompanyName__c,DealerInfo__c,BizLeaderRep__c,ExternalId__c,LastApproval__c from DABAN_BPM_DICS.dbo.V_MACUSER");
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, System.Data.CommandType.Text, sql).Tables[0];
            LogInfo.Log.Info("《SalesForceToDaBan》获取需处理数量：" + dt.Rows.Count + "条");
            if (dt.Rows.Count == 0)
                return;
            file_sb.AppendLine("Name__c\tEmail__c\tDICName__c\tArea__c\tBusinessType__c\tTitle__c\tCompanyName__c\tDealerInfo__c\tBizLeaderRep__c\tExternalId__c\tLastApproval__c");
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

        }
    }
}
