using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPIncome.SAP1
{
    public class SAPIncome : IncomePurchaseUpdateObject
    {
        //AIncomePurchaseUpdate aIncomePurchaseUpdate;
        public SAPIncome(string folderPath_Queue, string folderPath_Success, string folderPath_Faild, BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
            this.folderPath_Success = folderPath_Success;
            this.folderPath_Faild = folderPath_Faild;
            //this.sapType = SAPResultType.Pay;
        }

        public override void GetData()
        {
            aIncomePurchaseUpdate = AIncomePurchaseUpdate.Create(context.connStr, new IncomePurchaseUpdateType(IncomePurchaseUpdateType.Income));
            ConnectFile.connectState(folderPath_Queue, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            DirectoryInfo TheFolder = new DirectoryInfo(folderPath_Queue);
            foreach (FileInfo NextFile in TheFolder.GetFiles("ZSAF*"))
            {
                string sql = AggData(NextFile);
                Execute(sql, NextFile);
            }
            if (!string.IsNullOrEmpty(context.errMsg))
                LogInfo.Log.Info(context.errMsg);
            //回调
            aIncomePurchaseUpdate.ExecuteQuery(IsExecuteQuery);
        }
        private string AggData(FileInfo NextFile)
        {
            string company = main_Company_dic[NextFile.Name.Split('_')[1]];//ZACF00510_4010_20180306
            string fileName = NextFile.Name;//文件名称
            string fileDate = SplitDate(NextFile.Name.Split('_')[2].Split('.')[0]);//文件日期
            string str = string.Empty;
            using (StreamReader sr = new StreamReader(NextFile.FullName, Encoding.Default))
            {
                str = sr.ReadToEnd();
            }
            aIncomePurchaseUpdate.Add(company);
            //新增合同号 主表  20字符
            if (string.IsNullOrEmpty(str))
                return string.Empty;//空文本
            StringBuilder sb = new StringBuilder();
            string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string cbxz = string.Empty;//表头与表体关联键
            for (int i = 0; i < strlist.Length; i++)
            {
                string[] strs = strlist[i].Split('\t');
                if (!CheckDB(strs))
                {
                    context.errMsg = string.Format("第{0}条数据为D，尾号与正负奇偶不匹配", i + 1);
                    continue;
                }
                string bsf = strs[0];
                if (bsf == "H")
                {
                    //主表
                    bsf = strs[0].ToUpper();
                    cbxz= company + strs[0].ToUpper() + strs[1] + strs[2];//主键:公司+标识符+管理销售编号+正负
                    string xblnr = strs[1];
                    string zf = strs[2];
                    string bstzd = strs[3].ToUpper();
                    string bstzd2 = strs[4];
                    string vkorg = strs[5];
                    string vtweg = strs[6];
                    string kunnr = strs[7];
                    string name1 = strs[8];
                    string city1 = strs[9];
                    string city2 = strs[10];
                    string bstkd = strs[11];
                    string waerk = strs[12];
                    string prsdt = string.Empty;
                    if (strs[13] != "")
                        if (strs[13].Length == 8 && strs[13] != "00000000")
                            prsdt = SplitDate(strs[13]);
                        else prsdt = "";
                    else
                        prsdt = "";
                    string submi = strs[14];
                    string ihrez = strs[15];
                    string zzvkbur = strs[16];
                    string zzprctr = strs[17];
                    string zzjgy = strs[18];
                    string zzcntperson = strs[19];
                    string tel_number = strs[20];
                    string contractno = strs[21];
                    string JH = strs[22];
                    string ZRQF = strs[23];
                    string PDQF = strs[24];
                    string AZNY = strs[25];
                    string mail = string.Empty;
                    string mobile = string.Empty;
                    //26号之后添加邮箱、手机号2个字段
                    if (strs.Length > 26)
                    {
                        mail = strs[26];
                        mobile = strs[27];
                    }
                    string xblnr_prefix = XBLNRPrefixSuffix(1,strs)[0];
                    string xblnr_suffix = XBLNRPrefixSuffix(1,strs)[1];

                    //有合同号
                    sb.AppendLine(string.Format("INSERT INTO DABAN_BPM_{0}.DBO.MAIN_INCOME_H(CBXZ,CD,BSF,XBLNR,ZF,BSTZD,BSTZD2,VKORG,VTWEG,KUNNR,NAME1,CITY1,CITY2,BSTKD,WAERK,PRSDT,SUBMI,IHREZ,ZZVKBUR,ZZPRCTR,ZZJGY,ZZCNTPERSON,TEL_NUMBER,CONTRACTNO,JH,ZRQF,PDQF,AZNY,FILENAME,FILEDATE,XBLNR_Prefix,XBLNR_Suffix,MAIL,MOBILE) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}');", company, cbxz, company, bsf, xblnr, zf, bstzd, bstzd2, vkorg, vtweg, kunnr, name1, city1, city2, bstkd, waerk, prsdt, submi, ihrez, zzvkbur, zzprctr, zzjgy, zzcntperson, tel_number, contractno,JH, ZRQF, PDQF, AZNY, fileName, fileDate,xblnr_prefix,xblnr_suffix,mail,mobile));
                }
                else
                {
                    string bxp = strs[0].ToUpper() + strs[1] + strs[2];//主键:标识符+管理销售编号+明细号
                    //从表
                    bsf = strs[0].ToUpper();
                    string xblnr = strs[1];
                    string posnr = strs[2];
                    string matnr = strs[3];
                    string arktx = strs[4];
                    string werks = strs[5];
                    string zmeng = strs[6];
                    string zieme = strs[7];
                    string kbetr = strs[8];
                    string kbetr1 = strs[9];
                    string prctr = strs[10];
                    sb.AppendLine(string.Format("INSERT INTO DABAN_BPM_{0}.DBO.MAIN_INCOME_I(BXP,CBXZ,BSF,XBLNR,POSNR,MATNR,ARKTX,WERKS,ZMENG,ZIEME,KBETR,KBETR1,PRCTR) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},'{10}','{11}','{12}','{13}');", company, bxp, cbxz, bsf, xblnr, posnr, matnr, arktx, werks, zmeng, zieme, kbetr, kbetr1, prctr));
                }
            }

            return sb.ToString();
        }
        private Boolean CheckDB(string[] strs)
        {
            if (strs[3].ToUpper() == "D")
            {
                //维修类，末尾偶数，正负也是偶数，否则为无效数据
                //123456789012 3456
                int lastNum = Convert.ToInt32(strs[1].Substring(12, 4));
                if (strs[2] == "1")
                {
                    if (lastNum != 0 && lastNum % 2 == 1)
                        return true;
                    else
                        return false;
                }
                if (strs[2] == "0")
                {
                    if (lastNum == 0 || lastNum % 2 == 0)
                        return true;
                    else
                        return false;
                }
            }
            return true;
        }
        protected override string[] XBLNRPrefixSuffix(int xblnrIndex, string[] strs)
        {
            if (strs[3].ToUpper() == "D")
            {
                return new string[2] { strs[xblnrIndex].Substring(0, 12), strs[1].Substring(12, 4) };
            }
            if (strs[3].ToUpper() == "B")
            {
                return new string[2] { strs[xblnrIndex].Substring(0, 10), "" };
            }
            return null;
        }
    }
}
