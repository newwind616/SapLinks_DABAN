using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds
{
    public class DataConvert: BaseAction
    {
        /// <summary>
        /// 写入TSV的联携内容
        /// </summary>
        public StringBuilder file_sb = new StringBuilder();
        /// <summary>
        /// 更新SAP_COMPANYFUNDS_LINKS_QUEUE联携状态
        /// </summary>
        public StringBuilder upLinks_sql = new StringBuilder();
        /// <summary>
        /// 是否有需要联携的数据
        /// </summary>
        public Boolean boo = false;

        protected string Create(params string[] fields)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in fields)
            {
                sb.Append(item + "\t");
            }
            return sb.ToString().Substring(0, sb.ToString().LastIndexOf("\t"));
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
        /// <summary>
        /// 获取会计记账日期
        /// </summary>
        /// <param name="AccountDate">流程结束日期</param>
        /// <returns>会计期间日期</returns>
        protected DateTime GetAccountingDate(DateTime AccountDate)
        {
            DateTime tagAccountDate = Convert.ToDateTime(CompanyObject.MAIN_ACCOUNTINGPERIOD.Rows[0]["DATE"]);
            if (AccountDate > tagAccountDate)
                return tagAccountDate;
            else
                return AccountDate;
        }
        /// <summary>
        /// 科目转换
        /// </summary>
        /// <param name="account_No">会计科目</param>
        /// <param name="pay_obj_id">供应商编码</param>
        /// <param name="loan_Sign">借贷标志</param>
        /// <param name="NEWKO"></param>
        /// <param name="NEWBS"></param>
        /// <param name="NEWUM"></param>
        protected void ConvertAccount(string company,string pay_obj_id, string account_No, string loan_Sign,out string NEWKO, out string NEWBS, out string NEWUM)
        {
            DataRow[] drs = CompanyObject.SAP_CODECONVERT.Select(string.Format("LOAN_SIGN='{0}' AND SRCCODE='{1}' AND COMPANY='{2}'", loan_Sign, account_No,company));
            if (drs.Length == 1)
            {
                NEWKO = Convert.ToString(drs[0]["NEWKO"]) != "" ? Convert.ToString(drs[0]["NEWKO"]) : pay_obj_id;
                NEWBS = Convert.ToString(drs[0]["NEWBS"]);
                NEWUM = Convert.ToString(drs[0]["NEWUM"]);
            }
            else
            {
                NEWKO = account_No; ;
                NEWBS = loan_Sign == "借" ? "40" : "50";
                NEWUM = "";
            }
        }
        #region 添加客户行、供应商行后以下转换法废弃
        /// <summary>
        /// 根据供应商ID获取记帐代码
        /// </summary>
        /// <param name="pay_obj_id"></param>
        /// <returns></returns>
        protected string ConvertVenderToNEWBS(string pay_obj_id, string company)
        {
            if (pay_obj_id == "F99999999")
                return "31";
            DataTable dt = SQLHelper.ExecuteDataset(connStr, CommandType.Text, string.Format("SELECT SUPP_CODE FROM BPMDB.dbo.MAIN_SUPPLIER WHERE SUPP_ID='{0}' AND COMPANY='{1}'", pay_obj_id, company)).Tables[0];
            if (dt.Rows.Count == 0)
            {
                LogInfo.Log.Info("供应商：" + pay_obj_id + "不存在");
                return "39";
            }
            else
            {
                if (dt.Rows[0][0].ToString() == "2181020000" || dt.Rows[0][0].ToString() == "2181010000")
                    return "31";
                else
                    return "39";
            }
        } 
        
        /// <summary>
        /// 根据供应商ID获取特别总帐标志
        /// </summary>
        /// <param name="pay_obj_id"></param>
        /// <returns></returns>
        protected string ConvertVenderToNEWUM(string pay_obj_id,string company)
        {
            if (pay_obj_id == "F99999999")
                return "";
            DataTable dt = SQLHelper.ExecuteDataset(connStr, CommandType.Text, string.Format("SELECT SUPP_CODE FROM BPMDB.dbo.MAIN_SUPPLIER WHERE SUPP_ID='{0}' AND COMPANY='{1}'", pay_obj_id, company)).Tables[0];
            if (dt.Rows.Count == 0)
            {
                LogInfo.Log.Info("供应商：" + pay_obj_id + "不存在");
                return "H";
            }
            else
            {
                if (dt.Rows[0][0].ToString() == "2181020000" || dt.Rows[0][0].ToString() == "2181010000")
                    return "";
                else
                    return "H";
            }
            //if (pay_obj_id == "2181020000" || pay_obj_id == "2181010000")
            //    return "";
            //else
            //    return "H";
        }
        #endregion
    }
}
