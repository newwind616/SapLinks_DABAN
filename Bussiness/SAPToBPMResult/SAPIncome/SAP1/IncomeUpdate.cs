using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPIncome.SAP1
{
    public class IncomeUpdate: AIncomePurchaseUpdate
    {
        public IncomeUpdate(string connStr) : base(connStr)
        {
        }

        public override string UpdateSql(string company)
        {
            string query = @"USE [DABAN_BPM_{0}]
--更新最新销售单
--1.先全部改成非最新单据
UPDATE MAIN_INCOME_H SET ISNEWESTXBLNR=0
--2.更新最新单据成1
UPDATE AA SET AA.ISNEWESTXBLNR=BB.ISNEWESTXBLNR FROM  MAIN_INCOME_H AA,(SELECT(CD+BSF+XBLNR_PREFIX+MAX(XBLNR_SUFFIX)+ZF) CBXZ,1 ISNEWESTXBLNR  FROM MAIN_INCOME_H 
WHERE ZF=0 GROUP BY CD,BSF,XBLNR_PREFIX,ZF,BSTZD) BB WHERE AA.CBXZ=BB.CBXZ

--更新销售表客户名称
UPDATE AA SET AA.KHMC=BB.KHMC FROM MAIN_INCOME_H AA ,BPMDB.DBO.MAIN_CUST_TAX BB WHERE AA.KUNNR=BB.KH AND AA.CD=BB.CD

--更新采购表开票合计金额
--金额清0
UPDATE MAIN_INCOME_H SET KBETR=0,KPJE=0
--更新主表合计开票金额
UPDATE AA SET AA.KBETR=BB.KBETR,AA.KPJE=BB.KBETR FROM MAIN_INCOME_H AA,(
SELECT CBXZ,SUM(ABS(ISNULL(KBETR,0))) KBETR FROM MAIN_INCOME_I GROUP BY CBXZ
) BB WHERE AA.CBXZ=BB.CBXZ";
            return string.Format(query, company);
        }
    }
}
