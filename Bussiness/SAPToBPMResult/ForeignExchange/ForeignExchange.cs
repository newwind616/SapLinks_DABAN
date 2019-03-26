using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.ForeignExchange
{
    public class ForeignExchange : SAPToBPMResultObject
    {
        public ForeignExchange(string folderPath_Queue, string folderPath_Success, string folderPath_Faild, BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
            this.folderPath_Success = folderPath_Success;
            this.folderPath_Faild = folderPath_Faild;
        }

        //时间    美元  欧元  日元  港币  泰铢  马币
        public override void GetData()
        {
            string sql = "insert into MAIN_FOREIGN_EXCHANGE(DATE,USD,EUR,JPY,HKD,THB,MYR) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');";
            ConnectFile.connectState(folderPath_Queue, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            DirectoryInfo TheFolder = new DirectoryInfo(this.folderPath_Queue);
            foreach (FileInfo NextFile in TheFolder.GetFiles("dabanhl_*.txt"))
            {
                StringBuilder sb = new StringBuilder();
                try
                {
                    using (StreamReader sr = new StreamReader(NextFile.FullName, Encoding.UTF8))
                    {
                        string str = sr.ReadToEnd();
                        string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < strlist.Length; i++)
                        {
                            string[] strs = strlist[i].Split(',');
                            if (strs.Length != 5)
                            {
                                continue;
                            }
                            DateTime date = Convert.ToDateTime(strs[0]);
                            decimal USD = Convert.ToDecimal(strs[1]);
                            decimal EUR = Convert.ToDecimal(strs[2]);
                            decimal JPY = Convert.ToDecimal(strs[3]);
                            decimal HKD = Convert.ToDecimal(strs[4]);
                            decimal THB = Convert.ToDecimal(strs[5]);
                            decimal MYR = Convert.ToDecimal(strs[6]);
                            sb.AppendLine(string.Format(sql, date.ToShortDateString(), USD, EUR, JPY, HKD,THB,MYR));
                        }
                    }
                    Execute(sb.ToString(), NextFile);
                }
                catch (Exception ex)
                {
                    LogInfo.Log.Error(ex);
                    //发送邮件并移至错误文件夹中
                    context.MessageQueue("SAP联携异常", NextFile.FullName + "文件处理异常：" + ex.Message);
                    FileMove(NextFile, folderPath_Faild);
                }
            }
        }
    }
}
