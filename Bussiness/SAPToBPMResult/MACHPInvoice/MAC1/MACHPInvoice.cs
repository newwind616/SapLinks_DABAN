using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.MACHPInvoice.MAC1
{
    public class MACHPPInvoice : SAPToBPMResultObject
    {
        public MACHPPInvoice(string folderPath_Queue, string folderPath_Success, string folderPath_Faild, BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
            this.folderPath_Success = folderPath_Success;
            this.folderPath_Faild = folderPath_Faild;
        }

        //经销商编码	禀议号	红字发票信息表编号	金额	预定结算方式	地区
        public override void GetData()
        {
            /*ID	自增主键
ACCOUNT	支付经销商代码
BY_NO	禀议号
INV_NO	发票号
AMOUNT	金额
TYPE	支付经销商
AREA	地区
APPLY_NO	采贩号
DURATION	区间
FLAG	冻结标志*/
            string sql = "insert into BPMDB.dbo.MAIN_DIST_HP(FLAG,ACCOUNT,BY_NO,APPLY_NO,AMOUNT,TYPE,AREA,DURATION,ORDER_NO,ABSTRACT,CD) values (0,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');";
            ConnectFile.connectState(folderPath_Queue, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            //this.folderPath_Queue = @"D:\SourceCode\Files\Success";
            DirectoryInfo TheFolder = new DirectoryInfo(this.folderPath_Queue);
            foreach (FileInfo NextFile in TheFolder.GetFiles("SF_Discount_Certificates_TO_DABAN*.TSV"))
            {
                bool IsError = false;
                StringBuilder sb = new StringBuilder();
                try
                {
                    using (StreamReader sr = new StreamReader(NextFile.FullName, Encoding.UTF8))
                    {
                        string str = sr.ReadToEnd();
                        string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        List<string> APPLY_NOS = new List<string>();
                        for (int i = 1; i < strlist.Length; i++)
                        {
                            string[] strs = strlist[i].Split('\t');
                            if (strs.Length != 10)
                            {
                                continue;
                            }
                            string ACCOUNT = strs[0].Trim();
                            string BY_NO = strs[1].Trim();
                            string APPLY_NO = strs[2].Trim();
                            if (APPLY_NOS.Contains(APPLY_NO))
                            {
                                continue;
                            }
                            else {
                                APPLY_NOS.Add(APPLY_NO);
                            }
                            decimal AMOUNT = Convert.ToDecimal(strs[3].Trim());
                            string TYPE = strs[4].Trim();// 需检查是否为1,2,3其中一个
                            if (TYPE != "1" && TYPE != "2" && TYPE != "3")
                            {
                                LogInfo.Log.Error(string.Format("红字发票信息表编号【{0}】的预定结算方式（汇款/转货款）非1, 2, 3其中一个", APPLY_NO));
                                IsError = true;
                                continue;
                            }
                            string AREA = strs[5].Trim();
                            string DURATION = strs[6].Trim();
                            string ORDER_NO = strs[7].Trim();
                            string ABSTRACT = strs[8].Trim();
                            string COMPANY = strs[9].Trim();
                            sb.AppendLine(string.Format(sql, ACCOUNT, BY_NO, APPLY_NO, AMOUNT, TYPE, AREA, DURATION,ORDER_NO,ABSTRACT,COMPANY));
                        }
                    }
                    Execute(sb.ToString(), NextFile);
                }
                catch (Exception ex)
                {
                    LogInfo.Log.Error(ex);
                    IsError = true;
                    //发送邮件并移至错误文件夹中
                    context.MessageQueue("SAP联携异常", NextFile.FullName + "文件处理异常：" + ex.Message);
                    FileMove(NextFile, folderPath_Faild);
                }
                finally {
                    if (IsError)
                        context.MessageQueue("折扣折让回传处理异常", "折扣折让回传处理异常，详情请查看Log记录！");
                }
                
            }
        }
    }
}
