using SAPLinks.Helper.SaveFile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.ReceiptApply 
{
    public class RecEntity
    {
        T_SAP_COMPANYFUNDS_LINKS_QUEUE queue = new T_SAP_COMPANYFUNDS_LINKS_QUEUE();

        public void GetSqlByList(ref Dictionary<string, List<RecList>> dic, ref Dictionary<string, T_SAP_COMPANYFUNDS_LINKS_QUEUE> dicQ)
        {
            string company = "1020";
            string filePath = "DICS_Path_Rec".ToAppSetting();
            string fileName = "DICS_Name_Rec".ToAppSetting() + "_" + company + "_" + DateTime.Now.ToString("yyyyMMdd");
            string fileExt = "DICS_Ext_Rec".ToAppSetting();
            string sql = @"SELECT * FROM DABAN_BPM_DICS.DBO.V_DICS_SAPLINKFILEDATA A
                       INNER JOIN SAP_COMPANYFUNDS_LINKS_QUEUE B
                       ON A.SN = B.APPLY_NO AND ISLINK = 0";
            DataTable dt = SQLHelper.ExecuteDataset("BPMDB".ToConnectionString(), System.Data.CommandType.Text, sql).Tables[0];

            List<RecList> recList = new List<RecList>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                queue.Add(Convert.ToString(dt.Rows[i]["SN"]));
                getDtFor40(dt.Rows[i], recList);
                getDtFor31(dt.Rows[i], recList);
               
            }
            dic.Add("1020", recList);
            dicQ.Add("1020", queue);
            //AccvouchEntityConvertStreamBaseManager accvouchEntityConvertStreamBaseManager = new AccvouchEntityConvertStreamBaseManager();
            //SapLinksQueueEntity sapLinksQueue = new SapLinksQueueEntity("BPMDB".ToConnectionString(), queue.GetSql_ISLINK1());
            //StringBuilder sb = new StringBuilder();
            //foreach (RecList item in recList)
            //{
            //sb.AppendLine(item.ToVouchString());
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
            string bktxt = Convert.ToString(dr["MemberName"])+ " 王莉";
            string waers = "CNY";
            decimal wrbtr = Convert.ToDecimal(dr["Total"]);
            string sgtxt = Convert.ToString(dr["MemberName"]);

            string xref1 = Convert.ToString(dr["SN"]);
            if (xref1.IndexOf('Z') > 0)
                xref1 = xref1.Replace("DICS_Z", "");
            else
                xref1 = xref1.Replace("DICS_SJ", "");

            string payType = Convert.ToString(dr["payType"]);

            string BankName = Convert.ToString(dr["BankName"]);

            string newko = "";
            if (payType == "银行转账")
            {
                //银行存款 建设银行  1002010204
                //银行存款 中国银行  1002010102
                //银行存款 工商银行  1002010303
                //银行存款 交通银行  1002010405
                //银行存款 农业银行  1002010501
                //银行存款 上海银行  1002010601
                if (BankName == "建设银行")
                    newko = "1002010204";
                if (BankName == "中国银行")
                    newko = "1002010102";
                if (BankName == "工商银行")
                    newko = "1002010303";
                if (BankName == "交通银行")
                    newko = "1002010405";
                if (BankName == "农业银行")
                    newko = "1002010501";
                if (BankName == "上海银行")
                    newko = "1002010601";
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

        private void getDtFor31(DataRow dr, List<RecList> recList)
        {
            string newbs = Convert.ToString(dr["PKCODE"]);
            string nuwum = Convert.ToString(dr["SGLCODE"]);

            string xblnr = Convert.ToString(dr["SN"]);
            string bldat = Convert.ToDateTime(dr["FINDate"]).ToString("yyyyMMdd");
            string budat = Convert.ToDateTime(dr["PostingDate"]).ToString("yyyyMMdd");
            string bktxt = Convert.ToString(dr["MemberName"])+ " 王莉";
            string waers = "CNY";
            decimal wrbtr = Convert.ToDecimal(dr["Total"]);
            string sgtxt = Convert.ToString(dr["MemberName"]);
            string xref1 = Convert.ToString(dr["SN"]);
            if (xref1.IndexOf('Z') > 0)
                xref1 = xref1.Replace("DICS_Z", "");
            else
                xref1 = "";//xref1.Replace("DICS_SJ", "");

            string ApplyType = Convert.ToString(dr["ApplyType"]);
            string MemberNo = Convert.ToString(dr["MemberNo"]);
            string newko = "";

            //根据ApplyType + MemberNo前2位确定31科目。
            //1）MemberNo前2位为{ PB}，ApplyType为职工机特卖或空清特卖或空清，则31科目为
            //其他应付   职工机特卖 / 空清特卖 / 空清 A00020263；
            //2）MemberNo前2位为{ PF}，ApplyType为职工机特卖或空清特卖或空清，则31科目为
            //其他应付   职工机特卖 / 空清特卖 / 空清 A00020262
            //3）MemberNo前2位为{ PF}
            //            或【PB】，ApplyType为职工机，则
            //31科目为
            //其他应付   职工机 F20010056。
            //5）MemberNo为空，则获取custmerNo中的代码，生成31科目
            //预收账款（寄存款）+custmerNo.
            //6）ApplyType为重要客户，则31科目为
            //预收账款(寄存款)  A00020264
            if (ApplyType == "重要客户")
                newko = "A00020264";
            if (ApplyType == "职工机特卖" || ApplyType == "空清特卖" || ApplyType == "空清")
            {
                if (MemberNo == "" || MemberNo == null)
                {
                    string CustmerNo = Convert.ToString(dr["CustmerNo"]);
                    newko = CustmerNo;
                }
                else
                {
                    string title = MemberNo.Substring(0, 2);
                    if (title == "PB")
                        newko = "A00020263";
                    if (title == "PH")
                        newko = "A00020262";
                }
            }
            if (ApplyType == "职工机")
            {
                if (MemberNo == "" || MemberNo == null)
                {
                    string CustmerNo = Convert.ToString(dr["CustmerNo"]);
                    newko = CustmerNo;
                }
                else
                {
                    newko = "F20010056";
                }
            }
            if (ApplyType == "货款")
            {
                string CustmerNo = Convert.ToString(dr["CustmerNo"]);
                newko = CustmerNo;
            }
            if (ApplyType == "其他")
            {
                string CustmerNo = Convert.ToString(dr["Others"]);
                newko = CustmerNo;
            }

            RecList rec = new RecList();
            rec.XBLNR = xblnr;//参照号（XBLNR）
            rec.BLDAT = bldat;//凭证日期（BLDAT）
            rec.BUDAT = budat;//记账日期（BUDAT）
            rec.BKTXT = bktxt;//抬头摘要（BKTXT）
            rec.WAERS = waers;//币种（WAERS）
            rec.KURSF = "";//汇率（KURSF）
            rec.NEWKO = newko;//客户 / 供应商 / 会计科目代码（NEWKO）
            rec.NEWBS = newbs;//"31";//记账码（NEWBS）
            rec.NEWUM = nuwum;//特殊总账标识（NEWUM）
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
