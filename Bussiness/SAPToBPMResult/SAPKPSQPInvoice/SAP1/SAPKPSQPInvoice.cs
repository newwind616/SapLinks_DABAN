using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SAPKPSQPInvoice.SAP1
{
    public class SAPKPSQPInvoice : SAPToBPMResultObject
    {
        public SAPKPSQPInvoice(string folderPath_Queue, string folderPath_Success, string folderPath_Faild, BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
            this.folderPath_Success = folderPath_Success;
            this.folderPath_Faild = folderPath_Faild;
        }

        //公司代码	发票号	金额  会计凭证号	过帐日期	客户采购订单编号
        public override void GetData()
        {
            string sql = "insert into DABAN_BPM_{0}.dbo.MAIN_KPSQ_INVOICE(CD,INVOICECODE,AMOUNT,ACCVOUCHCODE,DATE,APPLY_NO) values ('{0}','{1}','{2}','{3}','{4}','{5}');";
            ConnectFile.connectState(folderPath_Queue, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            DirectoryInfo TheFolder = new DirectoryInfo(this.folderPath_Queue);
            foreach (FileInfo NextFile in TheFolder.GetFiles("INV_*.TSV"))
            {
                StringBuilder sb = new StringBuilder();
                try
                {
                    using (StreamReader sr = new StreamReader(NextFile.FullName, Encoding.UTF8))
                    {
                        string str = sr.ReadToEnd();
                        string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 1; i < strlist.Length; i++)
                        {
                            string[] strs = strlist[i].Split('\t');
                            if (strs.Length != 6)
                            {
                                continue;
                            }
                            string cd = main_Company_dic[strs[0]];//公司编码转换
                            string invoiceCode = strs[1];
                            decimal amount = Convert.ToDecimal(strs[2]);
                            string accvouchCode = strs[3];
                            DateTime date = Convert.ToDateTime(strs[4]);// Convert.ToDateTime(SplitDate(strs[4]));//20180904格式化2018-09-04
                            string apply_no = strs[5];
                            sb.AppendLine(string.Format(sql, cd, invoiceCode, amount, accvouchCode, date, apply_no));
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
