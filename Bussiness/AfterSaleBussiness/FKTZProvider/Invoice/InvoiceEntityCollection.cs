using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    /// <summary>
    /// 发票信息集合
    /// </summary>
    public class InvoiceEntityCollection:List<InvoiceEntity>
    {
        /// <summary>
        /// 发票类型
        /// </summary>
        public InvoiceType InvoiceType { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public decimal TaxTate { get; set;}
        /// <summary>
        /// 发票金额合计
        /// </summary>
        public decimal AmountTotal { get; set; }
        /// <summary>
        /// 发票税额合计
        /// </summary>
        public decimal TaxTotal { get; set; }
        public static InvoiceEntityCollection Load(ApplyNoEntity applyNoEntity)
        {
            return AggData(ExecuteQuery(applyNoEntity,StringFormat(applyNoEntity)));
        }
        private static string StringFormat(ApplyNoEntity applyNoEntity)
        {
            string query = @"select 'ZP' InvoiceType,INV_CODE,INV_NO,AMOUNT,TAX from DABAN_BPM_{0}.DBO.FKTZS_ZP_C_D where TASKID={1}
                            union all
                            select 'PP' InvoiceType,INV_CODE,INV_NO,AMOUNT,0 TAX from DABAN_BPM_{0}.DBO.FKTZS_PP_C_D where TASKID={1}";
            query = string.Format(query, applyNoEntity.BasicEntity.Company, applyNoEntity.TaskID);
            return query;
        }
        private static DataTable ExecuteQuery(ApplyNoEntity applyNoEntity,string query)
        {
            return SQLHelper.ExecuteDataset(applyNoEntity.BasicEntity.ConnStr, CommandType.Text, query).Tables[0];
        }
        private static InvoiceEntityCollection AggData(DataTable dt)
        {
            InvoiceEntityCollection invoices = new InvoiceEntityCollection();
            foreach (DataRow item in dt.Rows)
            {
                InvoiceEntity invoiceEntity = new InvoiceEntity();
                invoiceEntity.InvoiceType = InvoiceTypeFactory.Init(Convert.ToString(item["InvoiceType"]));
                invoiceEntity.Inv_Code = Convert.ToString(item["INV_CODE"]);
                invoiceEntity.Inv_No = Convert.ToString(item["INV_NO"]);
                invoiceEntity.Amount = Convert.ToDecimal(item["AMOUNT"]);
                invoices.AmountTotal += invoiceEntity.Amount;
                invoiceEntity.Tax = Convert.ToDecimal(item["TAX"]);
                invoices.TaxTotal+= invoiceEntity.Tax;
                invoices.Add(invoiceEntity);
            }
            //发票集合添加第一条发票的类型、税率
            if (invoices.Count != 0)
            {
                invoices.InvoiceType = invoices[0].InvoiceType;
                invoices.TaxTate = invoices[0].TaxRate;
            }
            return invoices;
        }
    }
}
