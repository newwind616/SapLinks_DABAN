using SAPLinks.Helper.SaveFile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.ReceiptApply
{
    public class JXBatchListEntity
    {
        
     
        public void SyncSap(ref Dictionary<string, List<RecList>> dic, ref Dictionary<string, T_SAP_COMPANYFUNDS_LINKS_QUEUE> dicQ)
        {
            string query = @"select A.*,(select top 1 SAP_CD   from  [BPMDB].[dbo].[MAIN_COMPANY] C where c.CD=A.Company) SAP_CD  from  DABAN_BPM_DICS.DBO.V_JX_BATCH_D A inner join 
[BPMDB].[dbo].[SAP_COMPANYFUNDS_LINKS_QUEUE] B on A.batch_No=b.APPLY_NO and B.ISLINK=0";
            DataTable dt = SQLHelper.ExecuteDataset("BPMDB".ToConnectionString(), System.Data.CommandType.Text, query).Tables[0];
            List<string> CompanyCodeList = new List<string>();
            CompanyCodeList.Add("1020");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string fromCompanyCode = Convert.ToString(dt.Rows[i]["SAP_CD"]);
                if (!CompanyCodeList.Contains(fromCompanyCode))
                {
                    CompanyCodeList.Add(fromCompanyCode);
                }
            }
            for (int j = 0; j < CompanyCodeList.Count; j++)
            {
                GetSqlByList(CompanyCodeList[j], dt.Select("SAP_CD='" + CompanyCodeList[j] + "'"), ref dic, ref dicQ);
            }
        }

        private void GetSqlByList(string companyCode, DataRow[] drs, ref Dictionary<string, List<RecList>> dic, ref Dictionary<string, T_SAP_COMPANYFUNDS_LINKS_QUEUE> dicQ)
        {
            T_SAP_COMPANYFUNDS_LINKS_QUEUE queue = new T_SAP_COMPANYFUNDS_LINKS_QUEUE();

            string filePath = "DICS_Path_Rec".ToAppSetting();
            string fileName = "DICS_Name_Rec".ToAppSetting() + "_" + companyCode + "_" + DateTime.Now.ToString("yyyyMMdd");
            string fileExt = "DICS_Ext_Rec".ToAppSetting();

            List<RecList> recList = new List<RecList>();
            if (dic.ContainsKey(companyCode))
            {
                recList = dic[companyCode];
            }
            if (dicQ.ContainsKey(companyCode))
            {
                queue = dicQ[companyCode];
            }
            for (int i = 0; i < drs.Length; i++)
            {
                queue.Add(Convert.ToString(drs[i]["BATCH_NO"]));
                getDtFor40(drs[i], recList);
                getDtFor50(drs[i], recList);

            }
            if (dic.ContainsKey(companyCode))
            {
                dic[companyCode] = recList;
            }
            else
            {
                dic.Add(companyCode, recList);
            }

            if (dicQ.ContainsKey(companyCode))
            {
                dicQ[companyCode] = queue;
            }
            else
            {
                dicQ.Add(companyCode, queue);
            }
            //AccvouchEntityConvertStreamBaseManager accvouchEntityConvertStreamBaseManager = new AccvouchEntityConvertStreamBaseManager();
            //SapLinksQueueEntity sapLinksQueue = new SapLinksQueueEntity("BPMDB".ToConnectionString(), queue.GetSql_ISLINK1());
            //StringBuilder sb = new StringBuilder();
            //foreach (RecList item in recList)
            //{
            //    sb.AppendLine(item.ToVouchString());
            //}
           
            //SAPLinks.Helper.SaveFile.ISaveFileProvider saveFileProvider = SapSaveFileProviderFactory.CreateSaveProvider(sapLinksQueue);
            //saveFileProvider.AddFileInfo(new SapSaveFileInfo(filePath, fileName + fileExt, sb));
            //saveFileProvider.SaveFile();
        }

        private void getDtFor40(DataRow dr, List<RecList> recList)
        {
            string xblnr = Convert.ToString(dr["BATCH_NO"]);
            string bldat = Convert.ToDateTime(dr["POSTING_DATE"]).ToString("yyyyMMdd");
            string budat = Convert.ToDateTime(dr["POSTING_DATE"]).ToString("yyyyMMdd");
            string bktxt = "解现";
            string waers = "CNY";
            decimal wrbtr = Convert.ToDecimal(dr["SumAmount"]);
            string sgtxt = "解现";
            string xref1 = xblnr;
       
            string BankName = Convert.ToString(dr["BANK_NAME"]);
            string newko = Convert.ToString(dr["NEWKO"]);
           

            RecList rec = new RecList();
            rec.XBLNR = xblnr;//参照号（XBLNR）
            rec.BLDAT = bldat;//凭证日期（BLDAT）
            rec.BUDAT = budat;//记账日期（BUDAT）
            rec.BKTXT = bktxt;//抬头摘要（BKTXT）
            rec.WAERS = waers;//币种（WAERS）
            rec.KURSF = "";//汇率（KURSF）
            rec.NEWKO = newko;//客户 / 供应商 / 会计科目代码（NEWKO）
            rec.NEWBS = "40";//记账码（NEWBS）
            rec.NEWUM = "";//特殊总账标识（NEWUM）
            rec.NEWBW = "";//資産取引タイプ（NEWBW）
            rec.KOSTL = "";//成本中心（KOSTL）
            rec.PRCTR = "";//利润中心（PRCTR）
            rec.PROJK = "";//WBS要素（PROJK）
            rec.AUFNR = "";//内部订单号（AUFNR）
            rec.WRBTR = wrbtr.ToString();//凭证货币金额（WRBTR）//专票对应税额
            rec.DMBTR = "";//本地货币金额（DMBTR）
            rec.MWSKZ = "";//税码（MWSKZ）
            rec.ZUONR = "";//分配（ZUONR）
            rec.SGTXT = sgtxt;//明細テキスト（SGTXT）
            rec.XREF1 = "";//取引先参照キー１（XREF1）
            rec.XREF2 = "";//取引先参照キー２（XREF2）
            rec.XREF3 = "";//取引先参照キー３（XREF3）
            rec.ZFBDT = "";//支払基準日（ZFBDT）
            rec.ZTERM = "";//支払条件（ZTERM）
            rec.ZLSCH = "";//支払方法（ZLSCH）
            rec.ZLSPR = "";//支払保留（ZLSPR）
            rec.ZBD1T = "";//日数（ZBD1T）
            rec.HBKID = "";//取引銀行ID（HBKID）
            rec.BVTYP = "";//銀行タイプ（BVTYP）
            rec.VALUT = "";//起算日（VALUT）
            rec.WDATE = "";//手形振出日（WDATE）
            rec.WBANK = "";//銀行住所（WBANK）
            recList.Add(rec);
        }

        private void getDtFor50(DataRow dr, List<RecList> recList)
        {
            string xblnr = Convert.ToString(dr["BATCH_NO"]);
            string bldat = Convert.ToDateTime(dr["POSTING_DATE"]).ToString("yyyyMMdd");
            string budat = Convert.ToDateTime(dr["POSTING_DATE"]).ToString("yyyyMMdd");
            string bktxt = "解现";
            string waers = "CNY";
            decimal wrbtr = Convert.ToDecimal(dr["SumAmount"]);
            string sgtxt = "解现";
            string xref1 = xblnr;

            string BankName = Convert.ToString(dr["BANK_NAME"]);
            string newko = "1001010100";

            RecList rec = new RecList();
            rec.XBLNR = xblnr;//参照号（XBLNR）
            rec.BLDAT = bldat;//凭证日期（BLDAT）
            rec.BUDAT = budat;//记账日期（BUDAT）
            rec.BKTXT = bktxt;//抬头摘要（BKTXT）
            rec.WAERS = waers;//币种（WAERS）
            rec.KURSF = "";//汇率（KURSF）
            rec.NEWKO = newko;//客户 / 供应商 / 会计科目代码（NEWKO）
            rec.NEWBS = "50";//记账码（NEWBS）
            rec.NEWUM = "";//特殊总账标识（NEWUM）T：寄存款
            rec.NEWBW = "";//資産取引タイプ（NEWBW）
            rec.KOSTL = "";//成本中心（KOSTL）
            rec.PRCTR = "";//利润中心（PRCTR）
            rec.PROJK = "";//WBS要素（PROJK）
            rec.AUFNR = "";//内部订单号（AUFNR）
            rec.WRBTR = wrbtr.ToString();//凭证货币金额（WRBTR）//专票对应税额
            rec.DMBTR = "";//本地货币金额（DMBTR）
            rec.MWSKZ = "";//税码（MWSKZ）
            rec.ZUONR = "";//分配（ZUONR）
            rec.SGTXT = sgtxt;//明細テキスト（SGTXT）
            rec.XREF1 = "";//取引先参照キー１（XREF1）
            rec.XREF2 = "";//取引先参照キー２（XREF2）
            rec.XREF3 = "";//取引先参照キー３（XREF3）
            rec.ZFBDT = "";//支払基準日（ZFBDT）
            rec.ZTERM = "";//支払条件（ZTERM）
            rec.ZLSCH = "";//支払方法（ZLSCH）
            rec.ZLSPR = "";//支払保留（ZLSPR）
            rec.ZBD1T = "";//日数（ZBD1T）
            rec.HBKID = "";//取引銀行ID（HBKID）
            rec.BVTYP = "";//銀行タイプ（BVTYP）
            rec.VALUT = "";//起算日（VALUT）
            rec.WDATE = "";//手形振出日（WDATE）
            rec.WBANK = "";//銀行住所（WBANK）
            recList.Add(rec);
        }



    }
}
