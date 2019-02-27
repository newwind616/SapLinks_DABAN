using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPDataToBPM.SAP1
{
    public class MAIN_SAP_FIN_ACCVOUCH : SAPDataToBPMObject
    {
        public MAIN_SAP_FIN_ACCVOUCH(string paths, string filters, BaseAction baseAction, Center_Subject subject) : base(paths, filters, baseAction, subject)
        {
        }

        public override void GetData()
        {
            //LogInfo.Log.Info("执行凭证数据回传");
            StringBuilder errMsg = new StringBuilder();
            StringBuilder successMsg = new StringBuilder();
            //SQLHelper.ExecuteNonQuery(context.connStr, "DELETE FROM MAIN_SUPPLIER WHERE SUPP_TYPE=0 AND COMPANY IN(" + filter + ")");
            StringBuilder sb_sql = new StringBuilder();
            //向表中添加数据
            ConnectFile.connectState(filePath, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
            {
                string str = sr.ReadToEnd();
                string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strlist.Length; i++)
                {
                    string[] strs = strlist[i].Split('\t');
                    if (strs.Length < 4)
                    {
                        errMsg.AppendLine("第" + (i + 1) + "行数据完整性异常");
                        errorCount++;
                        continue;
                    }
                    if (!dic.ContainsKey(strs[0]))
                    {
                        errMsg.AppendLine(string.Format("第{0}行公司编码:{1}不存在于MAIN_COMPANY表中", (i + 1), strs[0]));
                        errorCount++;
                        continue;
                    }
                    sb_sql.AppendLine(string.Format(@"insert into MAIN_SAP_FIN_ACCVOUCH(company,apply_no,voucher_no,account_date) 
                                                        values ('{0}','{1}','{2}','{3}');", dic[strs[0]], strs[2], strs[1], SplitDate(strs[3])));
                    successMsg.AppendLine(string.Format("第{0}行公司:{1}凭证组装成功", i + 1, dic[strs[0]]));
                    successCount++;
                }
            }
            try
            {
                SQLHelper.ExecuteNonQuery(context.connStr, sb_sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Log();
        }
        
    }
}
