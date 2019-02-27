using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.MACHPTYPEInvoice.MAC2
{
    public class MACHPTYPEInvoice : SAPToBPMResultObject
    {
        public MACHPTYPEInvoice(string folderPath_Queue, string folderPath_Success, string folderPath_Faild, BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
            this.folderPath_Success = folderPath_Success;
            this.folderPath_Faild = folderPath_Faild;
        }

        //红字发票信息表编号	
        public override void GetData()
        {
            /*ID 数据表主键
APPLY_NO 编号
FLAG 标识
TYPE 付款方式
CAT 类型 
INV_NO 发票号码
PAY_DATE 支付日期*/
            //MAIN_DIST_HP
           // string sql = "insert into DABAN_BPM_DICS.dbo.MAIN_DIST_HPTYPE(CAT,APPLY_NO,FLAG,TYPE,INV_NO,PAY_DATE) values ({0},{1},{2},{3},{4},{5});";
            string u_sql = "update BPMDB.dbo.MAIN_DIST_HP set FLAG='{0}',TYPE='{1}' where ORDER_NO='{2}'; ";
            //本地测试注释,正式环境释放(文件服务器连接)
            ConnectFile.connectState(folderPath_Queue, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());

            DirectoryInfo TheFolder = new DirectoryInfo(this.folderPath_Queue);
            foreach (FileInfo NextFile in TheFolder.GetFiles("SFD*.TSV"))
            {
                bool IsError = false;
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
                            string CAT = "'"+strs[0].Trim()+"'";
                            string ORDER_NO = "'" + strs[1].Trim() + "'";
                            //if (!IsExisit(strs[1].Trim())) {
                            //    LogInfo.Log.Error(string.Format("红字发票信息表编号【{0}】在 DABAN_BPM_DICS.dbo.MAIN_DIST_HP 中不存在！", strs[1].Trim()));
                            //    IsError = true;
                            //    continue;
                            //}
                            int FLAG = strs[2].Trim().ToUpper() == "X" ? 1 : 0;

                            string TYPE = "'" + strs[3].Trim() + "'";
                            string INV_NO = "'" + strs[4].Trim() + "'";
                            string PAY_DATE = "";
                            if (string.IsNullOrEmpty(strs[5].Trim()))
                            {
                                PAY_DATE = "NULL";
                            }
                            else
                            {
                                DateTime dt = DateTime.MinValue;
                                PAY_DATE = "'" + DateTime.TryParse(strs[5], out dt).ToString() + "'";
                            }
                            sb.AppendLine(string.Format(u_sql,FLAG, TYPE, ORDER_NO));
                        }
                    }
                    Execute(sb.ToString(), NextFile);
                }
                catch (Exception ex)
                {
                    LogInfo.Log.Error(ex);
                    IsError = true;
                    //发送邮件并移至错误文件夹中
                    context.MessageQueue("供应商红票状态回传", NextFile.FullName + "文件处理异常：" + ex.Message);
                    FileMove(NextFile, folderPath_Faild);
                }
                finally
                {
                    if (IsError)
                        context.MessageQueue("折扣折让状态回传处理异常", "折扣折让状态回传处理异常，详情请查看Log记录！");
                }

            }
        }

        private bool IsExisit(string applyNo) {
            string s = string.Format("select count(1) Cnt from DABAN_BPM_DICS.dbo.MAIN_DIST_HP where APPLY_NO='{0}';", applyNo);
            int cnt = Convert.ToInt32(SQLHelperExt.ExecuteFirstValue(context.connStr, s));
            if (cnt > 0)
                return true;
            else
                return false;
        }
    }
}
