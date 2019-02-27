using SAPLinks.Helper.SaveFile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.ReceiptApply 
{
    public class SpecialSaleEntity
    {
       
        public void SyncSap(ref Dictionary<string, List<RecList>> dic, ref Dictionary<string, T_SAP_COMPANYFUNDS_LINKS_QUEUE> dicQ) {
            string query = @"SELECT * FROM DABAN_BPM_DICS.DBO.V_EmptyAndClearSale_Post A
                       INNER JOIN SAP_COMPANYFUNDS_LINKS_QUEUE B
                       ON A.SN = B.APPLY_NO AND ISLINK = 0";
            DataTable dt = SQLHelper.ExecuteDataset("BPMDB".ToConnectionString(), System.Data.CommandType.Text, query).Tables[0];
            List<string> CompanyCodeList = new List<string>();
            CompanyCodeList.Add("1020");
            CompanyCodeList.Add("1010");
            CompanyCodeList.Add("1050");
            CompanyCodeList.Add("1030");
            CompanyCodeList.Add("1040");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string fromCompanyCode = Convert.ToString(dt.Rows[i]["FromCompanyCode"]);
                if (!CompanyCodeList.Contains(fromCompanyCode)) {
                    CompanyCodeList.Add(fromCompanyCode);
                }
            }
            for (int j = 0; j < CompanyCodeList.Count; j++)
            {
                GetSqlByList(CompanyCodeList[j],dt.Select("FromCompanyCode='"+ CompanyCodeList[j]+"'"), ref dic,ref dicQ);
            }
        }

        private void GetSqlByList(string companyCode,DataRow[] drs, ref Dictionary<string, List<RecList>> dic, ref Dictionary<string, T_SAP_COMPANYFUNDS_LINKS_QUEUE> dicQ)
        {
            T_SAP_COMPANYFUNDS_LINKS_QUEUE queue = new T_SAP_COMPANYFUNDS_LINKS_QUEUE();

            //companyCode = "1020";
            string filePath = "DICS_Path_Rec".ToAppSetting();
            string fileName = "DICS_Name_Rec".ToAppSetting() + "_" + companyCode + "_" + DateTime.Now.ToString("yyyyMMdd");
            string fileExt = "DICS_Ext_Rec".ToAppSetting();
            //string sql = @"SELECT * FROM DABAN_BPM_DICS.DBO.V_EmptyAndClearSale_Post A
            //           INNER JOIN SAP_COMPANYFUNDS_LINKS_QUEUE B
            //           ON A.SN = B.APPLY_NO AND ISLINK = 0";
            //DataTable dt = SQLHelper.ExecuteDataset("BPMDB".ToConnectionString(), System.Data.CommandType.Text, sql).Tables[0];

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
                queue.Add(Convert.ToString(drs[i]["SN"]));
                getDtFor40(drs[i], recList);
                getDtFor19(drs[i], recList);
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
            string xblnr = Convert.ToString(dr["SN"]);
            string bldat = Convert.ToDateTime(dr["FINDate"]).ToString("yyyyMMdd");
            string budat = Convert.ToDateTime(dr["PostingDate"]).ToString("yyyyMMdd");
            string bktxt = Convert.ToString(dr["BKTXT"]);
            string waers = "CNY";
            decimal wrbtr = Convert.ToDecimal(dr["Total"]);
            string sgtxt = bktxt.Substring(0, bktxt.IndexOf(")") + 1);//"寄存款(" + Convert.ToString(dr["ApplyType"]) + ")";
            string xref1 = "";// xblnr.Substring(xblnr.Length - 11, 11);//获取11位流水号 
           
            //string xref1 = Convert.ToString(dr["SN"]);
            //if (xref1.IndexOf('Z') > 0)
            //    xref1 = xref1.Replace("DICS_Z", "");
            //else
            //    xref1 = xref1.Replace("DICS_SJ", "");

            string payType = Convert.ToString(dr["payType"]);
            string BankName = Convert.ToString(dr["BankName"]);

            string newko = "";
            if (payType == "银行转账")
            {
                newko = Convert.ToString(dr["SapBankNO"]);
                //银行存款 建设银行  1002010204
                //银行存款 中国银行  1002010102
                //银行存款 工商银行  1002010303
                //银行存款 交通银行  1002010405
                //银行存款 农业银行  1002010501
                //银行存款 上海银行  1002010601
                //银行存款 招商银行  1002011202
                //if (BankName == "建设银行")
                //    newko = "1002010204";
                //if (BankName == "中国银行")
                //    newko = "1002010102";
                //if (BankName == "工商银行")
                //    newko = "1002010303";
                //if (BankName == "交通银行")
                //    newko = "1002010405";
                //if (BankName == "农业银行")
                //    newko = "1002010501";
                //if (BankName == "上海银行")
                //    newko = "1002010601";
                //if (BankName == "招商银行")
                //    newko = "1002011202";
            }
            else
            {
                 newko = "1001010100";
            }

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

        private void getDtFor19(DataRow dr, List<RecList> recList)
        {
            string xblnr = Convert.ToString(dr["SN"]);
            string bldat = Convert.ToDateTime(dr["FINDate"]).ToString("yyyyMMdd");
            string budat = Convert.ToDateTime(dr["PostingDate"]).ToString("yyyyMMdd");
            string bktxt = Convert.ToString(dr["BKTXT"]);
            string waers = "CNY";
            decimal wrbtr = Convert.ToDecimal(dr["Total"]);
            string sgtxt = bktxt.Substring(0,bktxt.IndexOf(")")+1); //"寄存款(" + Convert.ToString(dr["ApplyType"]) + ")";
            string xref1 = "";//xblnr.Substring(xblnr.Length - 11, 11);//获取11位流水号        

            string ApplyType = Convert.ToString(dr["ApplyType"]);
            string MemberNo = Convert.ToString(dr["MemberNo"]);
            string newko = Convert.ToString(dr["SapCusNo"]);

            

            RecList rec = new RecList();
            rec.XBLNR = xblnr;//参照号（XBLNR）
            rec.BLDAT = bldat;//凭证日期（BLDAT）
            rec.BUDAT = budat;//记账日期（BUDAT）
            rec.BKTXT = bktxt;//抬头摘要（BKTXT）
            rec.WAERS = waers;//币种（WAERS）
            rec.KURSF = "";//汇率（KURSF）
            rec.NEWKO = newko;//客户 / 供应商 / 会计科目代码（NEWKO）
            rec.NEWBS = "19";//记账码（NEWBS）
            rec.NEWUM = "T";//特殊总账标识（NEWUM）T：寄存款
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
            rec.XREF1 = xref1;//取引先参照キー１（XREF1）
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
