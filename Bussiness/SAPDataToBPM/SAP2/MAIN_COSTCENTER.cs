using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPDataToBPM.SAP2
{
    public class MAIN_COSTCENTER : SAPDataToBPMObject
    {
        public MAIN_COSTCENTER(string paths, string filters, BaseAction baseAction, Center_Subject center) : base(paths, filters, baseAction, center)
        {
        }

        public override void GetData()
        {
            LogInfo.Log.Info("执行SAP2成本中心主数据同步");
            StringBuilder errMsg = new StringBuilder();
            StringBuilder successMsg = new StringBuilder();
            SQLHelper.ExecuteNonQuery(context.connStr, "DELETE FROM MAIN_COSTCENTER WHERE COMPANY IN (" + filter + ")");
            //获得表结构和数据
            DataTable costcenter = SQLHelper.ExecuteDataset(context.connStr, CommandType.Text, "SELECT COMPANY, CODE, NAME, DEPARTMENT, DEPARTMENT_TYPE, DEL_FLG FROM [DBO].[MAIN_COSTCENTER] WHERE COMPANY IN (" + filter + ")").Tables[0];//GetColumnNames_MAIN_COSTCENTER();
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
                    if (!dic.ContainsKey(strs[0]))
                    {
                        errMsg.AppendLine(string.Format("第{0}行公司编码:{1}编码:{2}名称:{3}不存在于MAIN_COMPANY表中", (i + 1), strs[0], strs[1].TrimStart('0'), strs[2]));
                        errorCount++;
                        continue;
                    }
                    dr = costcenter.NewRow();
                    dr["COMPANY"] = dic[strs[0]];
                    dr["CODE"] = strs[1];
                    dr["NAME"] = strs[2];
                    dr["DEPARTMENT"] = strs[3];
                    dr["DEPARTMENT_TYPE"] = strs[4];
                    dr["DEL_FLG"] = strs[5].ToUpper() == "X" ? 1 : 0;
                    costcenter.Rows.Add(dr);
                    successMsg.AppendLine(string.Format("第{0}行公司:{1}编码:{2}名称:{3}部门:{4}部门类型:{5}成功", i + 1, dic[strs[0]], strs[1].TrimStart('0'), strs[2], strs[3], strs[4]));
                    successCount++;
                }
            }
            try
            {
                ExecuteMainData(costcenter, "MAIN_COSTCENTER");
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
