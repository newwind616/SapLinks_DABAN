using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult
{
    public abstract class AIncomePurchaseUpdate
    {
        protected string connStr { get; set; }
        public AIncomePurchaseUpdate(string connStr)
        {
            this.connStr = connStr;
        }
        protected StringBuilder sb = new StringBuilder();
        protected Dictionary<string, string> DataBaseName = new Dictionary<string, string>();
        public AIncomePurchaseUpdate Add(string databaseName)
        {
            if (!DataBaseName.ContainsKey(databaseName))
                DataBaseName.Add(databaseName, databaseName);
            return this;
        }
        public void ExecuteQuery(bool isExecuteQuery)
        {
            if (!isExecuteQuery)
                return;
            foreach (var item in DataBaseName)
            {
                sb.AppendLine(this.UpdateSql(item.Key));
            }
            if (string.IsNullOrEmpty(sb.ToString()))
                return;
            SqlCommand cmd = null;
            try
            {
                cmd = SQLHelper.GetTransactionSqlCommand(connStr);
                SQLHelper.ExecuteNonQuery(ref cmd, sb.ToString());
                cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                LogInfo.Log.Info("执行Income/Purchase数据更新失败,此错误不发邮件也不回滚" + ex.Message);
                cmd.Transaction.Rollback();
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Dispose();
            }
        }
        public abstract string UpdateSql(string company);
        public static AIncomePurchaseUpdate Create(string connStr,IncomePurchaseUpdateType type)
        {
            switch (type.Type)
            {
                case IncomePurchaseUpdateType.Income:
                    return new SAPIncome.SAP1.IncomeUpdate(connStr);
                case IncomePurchaseUpdateType.Purchase:
                    return new SAPPurchase.SAP1.PurchaseUpdate(connStr);
                default:
                    return null;
            }
        }
    }
    public class IncomePurchaseUpdateType
    {
        public const string Income = "Income";
        public const string Purchase = "Purchase";
        public string Type { get; set; }
        public IncomePurchaseUpdateType(string type)
        {
            this.Type = type;
        }
    }
}
