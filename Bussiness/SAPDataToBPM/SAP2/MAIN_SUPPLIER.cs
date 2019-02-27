using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPDataToBPM.SAP2
{
    public class MAIN_SUPPLIER : SAPDataToBPMObject
    {
        public MAIN_SUPPLIER(string paths, string filters, BaseAction baseAction, Center_Subject center) : base(paths, filters, baseAction, center)
        {
        }
        public override void GetData()
        {
            LogInfo.Log.Info("执行SAP2供应商行主数据同步");
            StringBuilder errMsg = new StringBuilder();
            StringBuilder successMsg = new StringBuilder();
            SQLHelper.ExecuteNonQuery(context.connStr, "DELETE FROM MAIN_SUPPLIER WHERE SUPP_TYPE=0 AND COMPANY IN(" + filter + ")");
            DataTable mainsupplier = SQLHelper.ExecuteDataset(context.connStr, CommandType.Text, "SELECT COMPANY,SUPP_ID,SUPP_NAME,SUPP_ABB,SUPP_TYPE,BANK_NAME,BANK_SUB_NAME,BANK_ACCOUNT,DEL_FLG,SUPP_CODE FROM MAIN_SUPPLIER WHERE COMPANY IN(" + filter + ")").Tables[0];
            //向表中添加数据
            DataRow dr;
            ConnectFile.connectState(filePath, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
            {
                string str = sr.ReadToEnd();
                string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strlist.Length; i++)
                {
                    string[] strs = strlist[i].Split('\t');
                    if (strs.Length < 6)
                    {
                        errMsg.AppendLine("第" + (i + 1) + "行数据完整性异常");
                        errorCount++;
                        continue;
                    }
                    if (!dic.ContainsKey(strs[0]))
                    {
                        errMsg.AppendLine(string.Format("第{0}行公司编码:{1}供应商编码:{2}供应商名称:{3}不存在于MAIN_COMPANY表中", (i + 1), strs[0], strs[1], strs[2]));
                        errorCount++;
                        continue;
                    }
                    if (strs[1].Substring(0, 1).ToUpper() != "P")
                    {
                        errMsg.AppendLine(string.Format("第{0}行公司编码:{1}供应商编码:{2}供应商名称:{3}非【P】开头账号", (i + 1), strs[0], strs[1], strs[2]));
                        errorCount++;
                        continue;
                    }
                    dr = mainsupplier.NewRow();
                    dr["COMPANY"] = dic[strs[0]];
                    dr["SUPP_ID"] = strs[1];
                    dr["SUPP_NAME"] = strs[2];
                    dr["SUPP_ABB"] = "";
                    dr["SUPP_TYPE"] = 0;//固定供应商
                    dr["BANK_NAME"] = strs[3];
                    dr["BANK_SUB_NAME"] = strs[3];
                    dr["BANK_ACCOUNT"] = strs[4];
                    if (strs[5].ToUpper() == "X")
                        continue;
                    dr["DEL_FLG"] = 0;
                    dr["SUPP_CODE"] = "";
                    mainsupplier.Rows.Add(dr);
                    successMsg.AppendLine(string.Format("第{0}行公司:{1}供应商编码:{2}供应商名称:{3}银行账号:{4}成功", i + 1, strs[0], strs[1], strs[2], strs[4]));
                    successCount++;
                }
            }
            try
            {
                ExecuteMainData(mainsupplier, "MAIN_SUPPLIER");
            }
            catch (Exception ex)
            {
                //LogInfo.Log.Error(ex);
                //exception = ex;
                throw ex;
            }
            Log();
        }
    }
}
