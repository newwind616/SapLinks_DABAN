using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPPay.SAP1
{
    public class SAP_PAY : SAPToBPMResultObject
    {
        public SAP_PAY(string folderPath_Queue, string folderPath_Success, string folderPath_Faild, BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
            this.folderPath_Success = folderPath_Success;
            this.folderPath_Faild = folderPath_Faild;
            this.sapType = SAPResultType.Pay;
        }

        public override void GetData()
        {
            ConnectFile.connectState(folderPath_Queue, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            DirectoryInfo TheFolder = new DirectoryInfo(folderPath_Queue);
            foreach (FileInfo NextFile in TheFolder.GetFiles("PAY*.TSV"))
            {
                if (!FileCheck(NextFile, "PAY"))
                {
                    context.MessageQueue(NextFile.FullName + "文件校验错误", NextFile.FullName + "文件校验错误");
                    FileMove(NextFile, folderPath_Faild);
                    break;
                }
                string sql = AggData(NextFile);
                Execute(sql, NextFile);
            }
        }
        private string AggData(FileInfo NextFile)
        {
            string str = string.Empty;
            using (StreamReader sr = NextFile.OpenText())
            {
                str = sr.ReadToEnd();
            }
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strlist.Length; i++)
            {
                string[] strs = strlist[i].Split('\t');
                string companycode = strs[0];
                string applyNo = strs[1];
                decimal apply_Amount = Convert.ToDecimal(strs[2]);
                string fileName = NextFile.Name;
                DateTime payDate =Convert.ToDateTime(strs[3]);
                DateTime fileDate = DateTime.ParseExact(NextFile.Name.ToUpper().Replace(".TSV", "").Split('_')[2], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                string company = string.Empty;
                if (main_Company_dic.ContainsKey(companycode))
                    company = main_Company_dic[companycode];
                sb.AppendLine(string.Format("INSERT INTO BPMDB.DBO.SAP_PAY(COMPANYCODE,COMPANY,APPLY_NO,APPLY_AMOUNT,PAYDATE,FILENAME,FILEDATE) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", companycode, company, applyNo, apply_Amount, payDate.ToString("yyyy-MM-dd HH:mm:ss"), fileName, fileDate.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            return sb.ToString();
        }
    }
}
