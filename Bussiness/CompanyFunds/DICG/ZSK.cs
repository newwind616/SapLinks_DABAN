using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds.DICG
{
    public class ZSK : DataConvert, IDataAction
    {
        public void GetData(DICG_Action dics_Action)
        {
            List<string> list = new List<string>();
            string sql = @"SELECT AA.APPLY_NO,AA.APPLY_DATE,AA.APPLY_DISPLAYNAME,AA.PAY_OBJ_TYPE,AA.PAY_OBJ_ID,AA.PAY_OBJ_NAME,AA.CURRENCY,AA.LOAN_AMOUNT,CC.ACCOUNT_DATE,
DBO.GETSAPFININFOCODE(AA.SAP_CUS_NEWBS,'',0) SAP_CUS_NEWBS,
DBO.GETSAPFININFOCODE(AA.SAP_CUST_NEWUM,'',0) SAP_CUST_NEWUM,
AA.SAP_CUST_CODE,
DBO.GETSAPFININFOCODE(AA.SAP_CUST_CODE,'DICG',2) CSR_ABB,
DBO.GETSAPFININFOCODE(AA.SAP_VEN_NEWBS,'',0) SAP_VEN_NEWBS,
DBO.GETSAPFININFOCODE(AA.SAP_VEN_NEWUM,'',0) SAP_VEN_NEWUM,
AA.SAP_VEN_CODE,
DBO.GETSAPFININFOCODE(AA.SAP_VEN_CODE,'DICG',1) SUPP_ABB,
AA.DK_PAY_OBJ_NAME,AA.PAY_MODE,
dbo.getFINDisplayName(AA.TASKID) FINDISPLAYNAME,
AA.ABSTRACT
FROM DABAN_BPM_DICG.DBO.ZSK_C_H AA
LEFT JOIN BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_QUEUE CC ON AA.APPLY_NO=CC.APPLY_NO
WHERE CC.COMPANY='DICG' AND CC.ISLINK=0 AND CC.ISLINK=0
ORDER BY CC.ID ASC,AA.APPLY_NO ASC";
            DataTable dt = SQLHelper.ExecuteDataset(connStr, System.Data.CommandType.Text, sql).Tables[0];
            LogInfo.Log.Info("《暂收款》获取需处理数量：" + dt.Rows.Count + "条");
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
                if (pay_obj_name.Length > 15)
                    pay_obj_name = pay_obj_name.Substring(0, 15);
                string supp_abb = Convert.ToString(dt.Rows[i]["SUPP_ABB"]);
                string csr_abb = Convert.ToString(dt.Rows[i]["CSR_ABB"]);
                string currency = Convert.ToString(dt.Rows[i]["CURRENCY"]);
                decimal loan_Amount = Convert.ToDecimal(dt.Rows[i]["LOAN_AMOUNT"]);
                string headerText = string.Empty;//凭证抬头文本
                //string newbs = ConvertVenderToNEWBS(pay_obj_id,dics_Action.company);
                //string newum = ConvertVenderToNEWUM(pay_obj_id, dics_Action.company);
                string sap_cus_newbs = Convert.ToString(dt.Rows[i]["SAP_CUS_NEWBS"]);//客户行记账码
                string sap_cust_newum = Convert.ToString(dt.Rows[i]["SAP_CUST_NEWUM"]);//客户行特别总账标识
                string sap_cust_code = Convert.ToString(dt.Rows[i]["SAP_CUST_CODE"]);//客户行编码
                string sap_ven_newbs = Convert.ToString(dt.Rows[i]["SAP_VEN_NEWBS"]);//供应商行记账码
                string sap_ven_newum = Convert.ToString(dt.Rows[i]["SAP_VEN_NEWUM"]);//供应商行特别总账标识
                string sap_ven_code = Convert.ToString(dt.Rows[i]["SAP_VEN_CODE"]);//供应商行编码
                string findisplayname = Convert.ToString(dt.Rows[i]["FINDISPLAYNAME"]);//财务初审姓名
                string bodyText = string.Empty;//凭证抬头文本带编码
                string _abstract = Convert.ToString(dt.Rows[i]["ABSTRACT"]);
                string _abstractABB = Convert.ToString(dt.Rows[i]["ABSTRACT"]);
                if (_abstractABB.Length > 15)
                    _abstractABB = _abstractABB.Substring(0, 15);
                headerText = string.Format("{0}借支{1} {2}", apply_DisplayName, _abstractABB, findisplayname);
                bodyText = string.Format("{0}借支{1} {2}", apply_DisplayName, _abstract, findisplayname);
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
                list.Add(loan_Amount.ToString());//凭证货币金额（WRBTR）									
                list.Add("");//国内通貨額（DMBTR）									
                list.Add("");//税代码（MWSKZ）									
                list.Add("");//分配编号（ZUONR）									
                list.Add(bodyText);//项目文本（SGTXT）									
                list.Add("");//参考码1（XREF1）									
                list.Add("");//参考码2（XREF2）									
                list.Add(apply_No);//参考码3（XREF3）									
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
                if ((i != dt.Rows.Count - 1 && dt.Rows[i]["APPLY_NO"].ToString() != dt.Rows[i + 1]["APPLY_NO"].ToString()) || (i == dt.Rows.Count - 1))
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
                    list.Add(loan_Amount.ToString());//凭证货币金额（WRBTR）									
                    list.Add("");//国内通貨額（DMBTR）									
                    list.Add("");//税代码（MWSKZ）									
                    list.Add("");//分配编号（ZUONR）									
                    list.Add(bodyText);//项目文本（SGTXT）									
                    list.Add("");//参考码1（XREF1）									
                    list.Add("");//参考码2（XREF2）									
                    list.Add(apply_No);//参考码3（XREF3）									
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
                    upLinks_sql.AppendLine(string.Format("UPDATE BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_QUEUE SET ISLINK=1 WHERE APPLY_NO='{0}'", apply_No));
                }
            }
        }
    }
}
