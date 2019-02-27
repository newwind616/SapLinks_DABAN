using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;


namespace SAPLinks.Bussiness.SAPToBPMResult.SAPPurchase.SAP1
{
    public class SAPPurchaseHD : SAPToBPMResultObject
    {
        public SAPPurchaseHD(string folderPath_Queue, string folderPath_Success, string folderPath_Faild, BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
            this.folderPath_Success = folderPath_Success;
            this.folderPath_Faild = folderPath_Faild;
            //this.sapType = SAPResultType.Pay;
        }
        public override void GetData()
        {
            ConnectFile.connectState(folderPath_Queue, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            DirectoryInfo TheFolder = new DirectoryInfo(folderPath_Queue);
            foreach (FileInfo NextFile in TheFolder.GetFiles("ZACF*"))
            {
                string sql = AggData(NextFile);
                Execute(sql, NextFile);
            }
        }
        private string AggData(FileInfo NextFile)
        {
            string company = main_Company_dic[NextFile.Name.Split('_')[1]];//ZACF00510_4010_20180306
            string fileName = NextFile.Name;//文件名称
            string fileDate = SplitDate(NextFile.Name.Split('_')[2].Split('.')[0]);//文件日期
            string str = string.Empty;
            string cxzbbb = string.Empty;
            using (StreamReader sr = new StreamReader(NextFile.FullName, Encoding.Default))
            {
                str = sr.ReadToEnd();
            }
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            StringBuilder sb_H = new StringBuilder();
            string sn = string.Empty;//上一步的作业单号
            int sn_zf = 0;//上一步的支付类别
            string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strlist.Length; i++)
            {

                //拼接sql
                string[] strs = strlist[i].Split('\t');
                string xblnr = strs[0];
                int zf = strs[1].ToUpper() == "1" ? 1 : 0; ;
                string bukrs = strs[2];
                string bldat = string.Empty;

                if (strs[3] != "")
                {
                    if (strs[3].Length == 8 && strs[3] != "00000000")
                        bldat = SplitDate(strs[3]);//date
                    else
                        bldat = "";
                }
                else
                    bldat = "";
                string budat = string.Empty;
                if (strs[4] != "")
                {
                    if (strs[4].Length == 8 && strs[4] != "00000000")
                        budat = SplitDate(strs[4]);//date
                    else
                        budat = "";
                }
                else
                    budat = "";
                string bktxt = strs[5];
                string waers = strs[6];
                string xref1_hd = strs[7];
                string xref2_hd = strs[8];
                string contractno = strs[9];
                string JH = strs[10];
                string ZRQF = strs[11];
                string PDQF = strs[12];
                string AZNY = strs[13];
                string saknr = strs[14];
                string kostl = strs[15];
                decimal wrbtr = Convert.ToDecimal(strs[16] == "" ? "0" : strs[16]);
                string zuonr = strs[17];
                string sgtxt = strs[18];
                string xref1 = strs[19];
                decimal xref2 = Convert.ToDecimal(strs[20] == "" ? "0" : strs[20]);
                decimal xref3 = Convert.ToDecimal(strs[21] == "" ? "0" : strs[21]);
                DateTime createDate = DateTime.Now;
                
                string bstzd = xref2_hd.Substring(0, 1).ToUpper();//DRBR区分
                if (i == 0)
                {
                    System.Threading.Thread.Sleep(1);//等待0.01秒,便于cxzbbb末尾号有差异
                    sn = xblnr;
                    sn_zf = zf;
                    cxzbbb = company + xblnr + zf + bukrs + strs[3] + strs[4] + createDate.ToString("yyyyMMddHHmmssfff");//主键
                    sb.AppendLine(string.Format("INSERT INTO DABAN_BPM_{0}.DBO.MAIN_PURCHASE_H(CXZBBB,CD,XBLNR,ZF,BUKRS,BLDAT,BUDAT,BKTXT,WAERS,XREF1_HD,XREF2_HD,CONTRACTNO,JH,ZRQF,PDQF,AZNY,FILENAME,FILEDATE,BSTZD,CREATEDATE) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}');", company, cxzbbb, company, xblnr, zf, bukrs, bldat, budat, bktxt, waers, xref1_hd, xref2_hd, contractno, JH, ZRQF, PDQF, AZNY, fileName, fileDate, bstzd, createDate.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                    sb.AppendLine(string.Format("INSERT INTO DABAN_BPM_{0}.DBO.MAIN_PURCHASE_I(CXZBBB,XBLNR,SAKNR,KOSTL,WRBTR,ZUONR,SGTXT,XREF1,XREF2,XREF3) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');", company, cxzbbb, xblnr, saknr, kostl, wrbtr, zuonr, sgtxt, xref1, xref2, xref3));
                }
                else
                {
                    if (xblnr + zf == sn + sn_zf)//作业单+正负标志保持连续唯一
                    {
                        sb.AppendLine(string.Format("INSERT INTO DABAN_BPM_{0}.DBO.MAIN_PURCHASE_I(CXZBBB,XBLNR,SAKNR,KOSTL,WRBTR,ZUONR,SGTXT,XREF1,XREF2,XREF3) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');", company, cxzbbb, xblnr, saknr, kostl, wrbtr, zuonr, sgtxt, xref1, xref2, xref3));
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1);//等待0.01秒,便于cxzbbb末尾号有差异
                        sn = xblnr;
                        sn_zf = zf;
                        cxzbbb = company + xblnr + zf + bukrs + strs[3] + strs[4] + createDate.ToString("yyyyMMddHHmmssfff");//主键
                        sb.AppendLine(string.Format("INSERT INTO DABAN_BPM_{0}.DBO.MAIN_PURCHASE_H(CXZBBB,CD,XBLNR,ZF,BUKRS,BLDAT,BUDAT,BKTXT,WAERS,XREF1_HD,XREF2_HD,CONTRACTNO,JH,ZRQF,PDQF,AZNY,FILENAME,FILEDATE,BSTZD,CREATEDATE) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}');", company, cxzbbb, company, xblnr, zf, bukrs, bldat, budat, bktxt, waers, xref1_hd, xref2_hd, contractno, JH, ZRQF, PDQF, AZNY, fileName, fileDate, bstzd, createDate.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                        sb.AppendLine(string.Format("INSERT INTO DABAN_BPM_{0}.DBO.MAIN_PURCHASE_I(CXZBBB,XBLNR,SAKNR,KOSTL,WRBTR,ZUONR,SGTXT,XREF1,XREF2,XREF3) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');", company, cxzbbb, xblnr, saknr, kostl, wrbtr, zuonr, sgtxt, xref1, xref2, xref3));
                    }
                }
            }
            return sb_H.AppendLine(sb.ToString()).ToString();
        }
    }
}
