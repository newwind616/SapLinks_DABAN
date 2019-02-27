using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPDataToBPM.SAP1
{
    public class MAIN_SUPPLIER : SAPDataToBPMObject
    {
        public MAIN_SUPPLIER(string paths, string filters, BaseAction baseAction, Center_Subject subject) : base(paths, filters, baseAction, subject)
        {
        }

        public override void GetData()
        {
            LogInfo.Log.Info("执行SAP1供应商行主数据同步");
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
                    if (strs.Length < 76)
                    {
                        errMsg.AppendLine("第" + (i + 1) + "行数据完整性异常");
                        errorCount++;
                        continue;
                    }
                    if (!dic.ContainsKey(strs[47]))
                    {
                        errMsg.AppendLine(string.Format("第{0}行公司编码:{1}供应商编码:{2}供应商名称:{3}不存在于MAIN_COMPANY表中", (i + 1), strs[47], strs[0], strs[2]));
                        errorCount++;
                        continue;
                    }
                    string key = dic[strs[47]] + strs[0];
                    dr = mainsupplier.NewRow();
                    dr["COMPANY"] = dic[strs[47]];
                    dr["SUPP_ID"] = strs[0];
                    dr["SUPP_NAME"] = strs[2];
                    dr["SUPP_ABB"] = strs[4];
                    if (string.IsNullOrEmpty(strs[0]))
                        dr["SUPP_TYPE"] = 1;//零星供应商
                    else
                        dr["SUPP_TYPE"] = 0;//固定供应商
                    dr["BANK_NAME"] = strs[76];
                    dr["BANK_SUB_NAME"] = strs[71];
                    dr["BANK_ACCOUNT"] = (strs[68] + strs[70]);
                    //删除数据不插入
                    if (strs[51].ToUpper() == "X")
                        continue;
                    dr["DEL_FLG"] = strs[51].ToUpper() == "X" ? 1 : 0;
                    dr["SUPP_CODE"] = strs[53];
                    mainsupplier.Rows.Add(dr);
                    successMsg.AppendLine(string.Format("第{0}行公司:{1}供应商编码:{2}供应商名称:{3}银行账号:{4}成功", i + 1, dic[strs[47]], strs[0], strs[2], (strs[68] + strs[70])));
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
