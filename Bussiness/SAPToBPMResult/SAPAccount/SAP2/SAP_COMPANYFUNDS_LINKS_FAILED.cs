using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPAccount.SAP2
{
    public class SAP_COMPANYFUNDS_LINKS_FAILED : SAPToBPMResultObject
    {
        public SAP_COMPANYFUNDS_LINKS_FAILED(string folderPath_Queue, string folderPath_Success, string folderPath_Faild, BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
            this.folderPath_Success = folderPath_Success;
            this.folderPath_Faild = folderPath_Faild;
            this.sapType = SAPResultType.Account;
        }
        public override void GetData()
        {
            ConnectFile.connectState(folderPath_Queue, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            DirectoryInfo TheFolder = new DirectoryInfo(folderPath_Queue);
            foreach (FileInfo NextFile in TheFolder.GetFiles("ERR*.TSV"))
            {
                if (!FileCheck(NextFile,"ERRORLOG"))
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
                string companycode = NextFile.Name.Split('_')[1].ToUpper();
                string applyNo = strs[0];
                DateTime errorDate = DateTime.ParseExact(NextFile.Name.ToUpper().Replace(".TSV", "").Split('_')[2], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                string errMessage = strs[1];
                string company = string.Empty;
                if (main_Company_dic.ContainsKey(companycode))
                    company = main_Company_dic[companycode];
                string fileName = NextFile.Name;
                sb.AppendLine(string.Format("INSERT INTO BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_FAILED(COMPANYCODE,COMPANY,APPLY_NO,DATE,MESSAGE,LINKS_TYPE,FILENAME) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", companycode, company, applyNo, errorDate.ToString("yyyy-MM-dd HH:mm:ss"), errMessage, "0", fileName));
            }
            return sb.ToString();
        }


    }
}