using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public abstract class AfterSaleObject
    {
        protected bool mailMobile = true;//添加邮件手机号
        protected string company { get; set; }
        protected string sap_cd { get; set; }
        protected BaseAction context { get; set; }
        protected string SalePrefix { get; set; }
        protected string PurchasePrefix { get; set; }
        protected string FileSuffix { get; set; }//TSV
        public int KPSQSaleCount = 0;//开票申请销售数量
        public int KPSQPurchaseCount = 0;//开票申请采购数量
        public int KPSQCXSaleCount = 0;//开票申请冲销销售数量
        public int KPSQCXPurchaseCount = 0;//开票申请冲销采购数量
        public int KPSQNotDcosSaleCount = 0;//开票申请非DCOS销售数量
        public int KPSQNotDcosPurchaseCount = 0;//开票申请非DCOS采购数量
        /// <summary>
        /// 生成文件路径
        /// </summary>
        public string _filePath { get; set; }
        protected string FilePath
        {
            get
            {
                if (_filePath.LastIndexOf('\\') == _filePath.Length - 1)
                    return _filePath;
                else
                    return _filePath + "\\";
            }
        }
        public AfterSaleObject(string filePath, string salePrefix, string prPrefix, string fileSuffix,string company,string sap_cd, BaseAction baseAction)
        {
            this._filePath = filePath;
            this.SalePrefix = salePrefix;
            this.PurchasePrefix = prPrefix;
            this.FileSuffix = fileSuffix;
            this.context = baseAction;
            this.company = company;
            this.sap_cd = sap_cd;
            this.BasicDataLoad();
        }
        protected string Create(List<string> fields)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in fields)
            {
                sb.Append(item + "\t");
            }
            return sb.ToString().Substring(0, sb.ToString().LastIndexOf("\t"));
        }
        protected void SaveToKPSQ(string sql, string saleData, string purchaseData)
        {
            DateTime time = DateTime.Now;
            //string saleFileName = string.Format("{0}_{1}_{2}{3}", SalePrefix, sap_cd, time, FileSuffix);
            //string purchaseFileName = string.Format("{0}_{1}_{2}{3}", PurchasePrefix, sap_cd, time, FileSuffix);
            string saleFileName = GetSaleFileName(time);
            string purchaseFileName = GetPurchaseFileName(time);
            if (string.IsNullOrEmpty(sql))
            {
                MainFile.WriteFile(FilePath, saleFileName, saleData, Encoding.Default);
                MainFile.WriteFile(FilePath, purchaseFileName, purchaseData);
            }
            else
            {
                SqlCommand cmd = SQLHelper.GetTransactionSqlCommand(context.connStr);
                try
                {
                    SQLHelper.ExecuteNonQuery(ref cmd, sql);
                    cmd.Transaction.Commit();
                    MainFile.WriteFile(FilePath, saleFileName, saleData, Encoding.Default);
                    MainFile.WriteFile(FilePath, purchaseFileName, purchaseData);
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
        }
        /// <summary>
        /// 销售文件名
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected virtual string GetSaleFileName(DateTime time)
        {
            string saleFileName = string.Format("{0}_{1}_{2}{3}", SalePrefix, sap_cd, time.ToString("yyyyMMddHHmm"), FileSuffix);
            return saleFileName;
        }
        /// <summary>
        /// 采购文件名
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected virtual string GetPurchaseFileName(DateTime time)
        {
            string purchaseFileName = string.Format("{0}_{1}_{2}{3}", PurchasePrefix, sap_cd, time.ToString("yyyyMMddHHmm"), FileSuffix);
            return purchaseFileName;
        }
        private Dictionary<string, string> kostConvertDic = new Dictionary<string, string>();
        private void BasicDataLoad()
        {
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, CommandType.Text, string.Format("select SRCKOSTL,TAGKOSTL from BPMDB.[dbo].[SAP_KOSTLCONVERT] where CD='{0}'", company)).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                if (!kostConvertDic.ContainsKey(dr["SRCKOSTL"].ToString()))
                    kostConvertDic.Add(dr["SRCKOSTL"].ToString(), dr["TAGKOSTL"].ToString());
            }
        }
        protected string GetKostl(string kostl)
        {
            if (kostConvertDic.ContainsKey(kostl))
                return kostConvertDic[kostl];
            else
                return kostl;
        }
        abstract public void GetData();
        protected virtual string[] GetPurchaseFilterSaknr()
        {
            string[] strs ={ "G002011", "G002012", "G002013", "G002014", "G002015", "G002017", "G152004", "G152008", "G152002", "G152003", "G152006", "G152003" };
            return strs;
        }
        protected virtual Boolean VerificationPurchaseSaknr(string saknr)
        {
            if (!GetPurchaseFilterSaknr().Contains(saknr))
                return true;
            return false;
        }
    }
}
