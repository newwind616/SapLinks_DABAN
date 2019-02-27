using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DSCG
{
    public class FKTZS_ZF : DataConvert, IDataAction
    {
        public void GetData(DSCG_Action dics_Action)
        {
            List<string> list = new List<string>();
            string sql = @"SELECT AA.APPLY_NO,AA.APPLY_DATE,AA.APPLY_DISPLAYNAME,AA.PAY_OBJ_TYPE,AA.PAY_OBJ_ID,AA.PAY_OBJ_NAME,AA.CURRENCY,CC.ACCOUNT_DATE,AA.APPLY_AMOUNT,
BB.LOAN_SIGN,BB.ACCOUNT_NO,BB.COSTCENTER_NO,BB.DISTRIBUTION,BB.ORDER_NO,BB.CAREER,BB.CREDIT_AMOUNT,
DBO.GETSAPFININFOCODE(AA.SAP_CUS_NEWBS,'',0) SAP_CUS_NEWBS,
DBO.GETSAPFININFOCODE(AA.SAP_CUST_NEWUM,'',0) SAP_CUST_NEWUM,
AA.SAP_CUST_CODE,
DBO.GETSAPFININFOCODE(AA.SAP_CUST_CODE,'DSCG',2) CSR_ABB,
DBO.GETSAPFININFOCODE(AA.SAP_VEN_NEWBS,'',0) SAP_VEN_NEWBS,
DBO.GETSAPFININFOCODE(AA.SAP_VEN_NEWUM,'',0) SAP_VEN_NEWUM,
AA.SAP_VEN_CODE,
DBO.GETSAPFININFOCODE(AA.SAP_VEN_CODE,'DSCG',1) SUPP_ABB,
AA.DK_PAY_OBJ_NAME,AA.PAY_MODE,
dbo.getFINDisplayName(AA.TASKID) FINDISPLAYNAME,
AA.ABSTRACT
FROM DABAN_BPM_DSCG.DBO.FKTZS_C_H AA
LEFT JOIN BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_QUEUE CC ON AA.APPLY_NO=CC.APPLY_NO
LEFT JOIN DABAN_BPM_DSCG.DBO.FKTZS_KM_C_D BB ON AA.TASKID=BB.TASKID
WHERE AA.PROCESS_TYPE=0 AND CC.ISLINK=0
ORDER BY CC.ID ASC,AA.APPLY_NO ASC,BB.ID ASC";
            DataTable dt = SQLHelper.ExecuteDataset(connStr, System.Data.CommandType.Text, sql).Tables[0];
            LogInfo.Log.Info("《付款通知书（支付）》获取需处理数量：" + dt.Rows.Count + "条");
            if (dt.Rows.Count == 0)
                return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Clear();
                string apply_No = Convert.ToString(dt.Rows[i]["APPLY_NO"]);
                DateTime account_Date = Convert.ToDateTime(dt.Rows[i]["ACCOUNT_DATE"]);
                DateTime apply_Date = Convert.ToDateTime(dt.Rows[i]["APPLY_DATE"]);
                string apply_DisplayName = Convert.ToString(dt.Rows[i]["APPLY_DISPLAYNAME"]);
                string pay_obj_type = Convert.ToString(dt.Rows[i]["PAY_OBJ_TYPE"]);
                //string pay_obj_id = Convert.ToString(dt.Rows[i]["PAY_OBJ_ID"]);
                //if (pay_obj_type == "1")
                //    pay_obj_id = "F99999999";//零星供应商固定编码
                string pay_obj_name = Convert.ToString(dt.Rows[i]["PAY_OBJ_NAME"]);
                if (pay_obj_name.Length > 10)
                    pay_obj_name = pay_obj_name.Substring(0, 10);
                string currency = Convert.ToString(dt.Rows[i]["CURRENCY"]);
                decimal apply_Amount = Convert.ToDecimal(dt.Rows[i]["APPLY_AMOUNT"]);
                string loan_Sign = Convert.ToString(dt.Rows[i]["LOAN_SIGN"]);
                string account_No = Convert.ToString(dt.Rows[i]["ACCOUNT_NO"]);
                string distribution = Convert.ToString(dt.Rows[i]["DISTRIBUTION"]);
                string costcenter_No = Convert.ToString(dt.Rows[i]["COSTCENTER_NO"]); 
                string order_No = Convert.ToString(dt.Rows[i]["ORDER_NO"]).ToFormatApply_No();//订单号
                string career = Convert.ToString(dt.Rows[i]["CAREER"]);
                decimal credit_Amount = Convert.ToDecimal(dt.Rows[i]["CREDIT_AMOUNT"]);
                //string supp_abb = Convert.ToString(dt.Rows[i]["SUPP_ABB"]);
                string supp_abb = Convert.ToString(dt.Rows[i]["SUPP_ABB"]);//供应商简称
                string sap_ven_newbs = Convert.ToString(dt.Rows[i]["SAP_VEN_NEWBS"]);//供应商行记账码
                string sap_ven_newum = Convert.ToString(dt.Rows[i]["SAP_VEN_NEWUM"]);//供应商行特别总账标识
                string sap_ven_code = Convert.ToString(dt.Rows[i]["SAP_VEN_CODE"]);//供应商行编码
                string csr_abb = Convert.ToString(dt.Rows[i]["CSR_ABB"]);//客户行简称
                string sap_cus_newbs = Convert.ToString(dt.Rows[i]["SAP_CUS_NEWBS"]);//客户行记账码
                string sap_cust_newum = Convert.ToString(dt.Rows[i]["SAP_CUST_NEWUM"]);//客户行特别总账标识
                string sap_cust_code = Convert.ToString(dt.Rows[i]["SAP_CUST_CODE"]);//客户行编码
                string findisplayname = Convert.ToString(dt.Rows[i]["FINDISPLAYNAME"]);//财务初审姓名
                string headerText = string.Empty;//凭证抬头文本带财务初审
                string bodyText = string.Empty;//凭证抬头文本带编码
                string _abstract = Convert.ToString(dt.Rows[i]["ABSTRACT"]);
                //if (pay_obj_type == "0")
                //{
                bodyText = string.Format("{0}支付{1}：{2}", apply_DisplayName, pay_obj_name, _abstract);
                string headerpay_obj_name = pay_obj_name;
                if (headerpay_obj_name.Length > 6)
                    headerpay_obj_name = headerpay_obj_name.Substring(0, 6);
                string headerabstract = _abstract;
                if (headerabstract.Length > 10)
                    headerabstract = headerabstract.Substring(0, 10);
                string headerapply_DisplayName = apply_DisplayName;
                if (headerapply_DisplayName.Length > 3)
                    headerapply_DisplayName = headerapply_DisplayName.Substring(0, 3);
                headerText = string.Format("{0}支付{1}{2} {3}", headerapply_DisplayName,headerpay_obj_name, headerabstract, findisplayname);
                //bodyText = string.Format("{0}{1}申请{2}{3}费用", apply_DisplayName, apply_Date.ToString("yyyy年MM月").Substring(2), supp_abb + csr_abb, sap_ven_code + sap_cust_code);
                //}
                //else
                //{
                //    headerText = string.Format("{0}{1}申请{2}费用 {3}", apply_DisplayName, apply_Date.ToString("yyyy年MM月").Substring(2), pay_obj_name, findisplayname);
                //    bodyText = string.Format("{0}{1}申请{2}{3}费用", apply_DisplayName, apply_Date.ToString("yyyy年MM月").Substring(2), pay_obj_name, sap_ven_code + sap_cust_code);
                //}
                string NEWKO, NEWBS, NEWUM;
                ConvertAccount(dics_Action.company, sap_ven_code, account_No, loan_Sign, out NEWKO, out NEWBS, out NEWUM);
                list.Add(apply_No);//参考凭证号（XBLNR）									
                list.Add(GetAccountingDate(account_Date).ToString("yyyyMMdd"));//凭证日期（BLDAT）									
                list.Add(GetAccountingDate(account_Date).ToString("yyyyMMdd"));//过帐日期（BUDAT）									
                list.Add(headerText);//凭证抬头文本（BKTXT）									
                list.Add(currency);//货币码（WAERS）									
                list.Add("");//汇率（KURSF）
                list.Add(NEWKO);//行项目的科目或匹配码（NEWKO）									
                list.Add(NEWBS);//记帐代码（NEWBS）									
                list.Add(NEWUM);//特别总帐标志（NEWUM）									
                list.Add("");//资产交易类型（NEWBW）									
                list.Add(costcenter_No);//成本中心（KOSTL）									
                list.Add("");//利润中心（PRCTR）									
                list.Add("");//项目科目分配（PROJK）									
                list.Add(order_No);//订单编号（AUFNR）									
                list.Add(credit_Amount.ToString());//凭证货币金额（WRBTR）									
                list.Add("");//国内通貨額（DMBTR）									
                list.Add("");//税代码（MWSKZ）									
                list.Add(distribution);//分配编号（ZUONR）									
                list.Add(bodyText);//项目文本（SGTXT）									
                list.Add("");//参考码1（XREF1）									
                list.Add(career);//参考码2（XREF2）									
                list.Add("");//参考码3（XREF3）									
                list.Add("");//支付基准日日（ZFBDT）									
                list.Add("");//付款条件代码（ZTERM）									
                list.Add("");//付款方式（ZLSCH）									
                list.Add("");//收付冻结码（ZLSPR）									
                list.Add("");//现金贴现日（ZBD1T）									
                list.Add("");//开户银行的简要键（HBKID）									
                list.Add("");//银行类型（BVTYP）									
                list.Add("");//起息日（VALUT）									
                list.Add("");//汇票签发日（WDATE）									
                list.Add("");//银行地址(WBANK）									
                file_sb.AppendLine(Create(list));
                if ((i != dt.Rows.Count- 1 && dt.Rows[i]["APPLY_NO"].ToString() != dt.Rows[i + 1]["APPLY_NO"].ToString()) ||(i== dt.Rows.Count - 1))
                {
                    if (!string.IsNullOrEmpty(sap_ven_code))
                    {
                        list.Clear();
                        list.Add(apply_No);//参考凭证号（XBLNR）									
                        list.Add(GetAccountingDate(account_Date).ToString("yyyyMMdd"));//凭证日期（BLDAT）									
                        list.Add(GetAccountingDate(account_Date).ToString("yyyyMMdd"));//过帐日期（BUDAT）									
                        list.Add(headerText);//凭证抬头文本（BKTXT）
                        list.Add(currency);//货币码（WAERS）									
                        list.Add("");//汇率（KURSF）
                        list.Add(sap_ven_code);//行项目的科目或匹配码（NEWKO）									
                        list.Add(sap_ven_newbs);//记帐代码（NEWBS）									
                        list.Add(sap_ven_newum);//特别总帐标志（NEWUM）									
                        list.Add("");//资产交易类型（NEWBW）									
                        list.Add("");//成本中心（KOSTL）									
                        list.Add("");//利润中心（PRCTR）									
                        list.Add("");//项目科目分配（PROJK）									
                        list.Add("");//订单编号（AUFNR）									
                        list.Add(apply_Amount.ToString());//凭证货币金额（WRBTR）									
                        list.Add("");//国内通貨額（DMBTR）									
                        list.Add("");//税代码（MWSKZ）									
                        list.Add("");//分配编号（ZUONR）									
                        list.Add(bodyText);//项目文本（SGTXT）									
                        list.Add("");//参考码1（XREF1）									
                        list.Add("");//参考码2（XREF2）									
                        list.Add("");//参考码3（XREF3）									
                        list.Add(account_Date.ToString("yyyyMMdd"));//支付基准日日（ZFBDT）									
                        list.Add("");//付款条件代码（ZTERM）									
                        list.Add("");//付款方式（ZLSCH）									
                        list.Add("");//收付冻结码（ZLSPR）									
                        list.Add("");//现金贴现日（ZBD1T）									
                        list.Add("");//开户银行的简要键（HBKID）									
                        list.Add("");//银行类型（BVTYP）									
                        list.Add("");//起息日（VALUT）									
                        list.Add("");//汇票签发日（WDATE）									
                        list.Add("");//银行地址(WBANK）									
                        file_sb.AppendLine(Create(list));
                    }
                    if (!string.IsNullOrEmpty(sap_cust_code))
                    {
                        list.Clear();
                        list.Add(apply_No);//参考凭证号（XBLNR）									
                        list.Add(GetAccountingDate(account_Date).ToString("yyyyMMdd"));//凭证日期（BLDAT）									
                        list.Add(GetAccountingDate(account_Date).ToString("yyyyMMdd"));//过帐日期（BUDAT）									
                        list.Add(headerText);//凭证抬头文本（BKTXT）
                        list.Add(currency);//货币码（WAERS）									
                        list.Add("");//汇率（KURSF）
                        list.Add(sap_cust_code);//行项目的科目或匹配码（NEWKO）									
                        list.Add(sap_cus_newbs);//记帐代码（NEWBS）									
                        list.Add(sap_cust_newum);//特别总帐标志（NEWUM）									
                        list.Add("");//资产交易类型（NEWBW）									
                        list.Add("");//成本中心（KOSTL）									
                        list.Add("");//利润中心（PRCTR）									
                        list.Add("");//项目科目分配（PROJK）									
                        list.Add("");//订单编号（AUFNR）									
                        list.Add(apply_Amount.ToString());//凭证货币金额（WRBTR）									
                        list.Add("");//国内通貨額（DMBTR）									
                        list.Add("");//税代码（MWSKZ）									
                        list.Add("");//分配编号（ZUONR）									
                        list.Add(bodyText);//项目文本（SGTXT）									
                        list.Add("");//参考码1（XREF1）									
                        list.Add("");//参考码2（XREF2）									
                        list.Add("");//参考码3（XREF3）									
                        list.Add(account_Date.ToString("yyyyMMdd"));//支付基准日日（ZFBDT）									
                        list.Add("");//付款条件代码（ZTERM）									
                        list.Add("");//付款方式（ZLSCH）									
                        list.Add("");//收付冻结码（ZLSPR）									
                        list.Add("");//现金贴现日（ZBD1T）									
                        list.Add("");//开户银行的简要键（HBKID）									
                        list.Add("");//银行类型（BVTYP）									
                        list.Add("");//起息日（VALUT）									
                        list.Add("");//汇票签发日（WDATE）									
                        list.Add("");//银行地址(WBANK）									
                        file_sb.AppendLine(Create(list));
                    }
                    upLinks_sql.AppendLine(string.Format("update BPMDB.dbo.SAP_COMPANYFUNDS_LINKS_QUEUE set ISLINK=1 where APPLY_NO='{0}'", apply_No));
                }
            }

        }
    }
}
