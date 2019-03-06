using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace SAPLinks.Bussiness.EDIDataToDABAN.EDI
{
    public class MAIN_EDI_DATA : EDIDataToDABANObject
    {
        public MAIN_EDI_DATA(string paths, string folderPath_Queue, BaseAction baseAction, Center_Subject subject) : base(paths, baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
        }

        public override void GetData()
        {
            LogInfo.Log.Info("执行EDI行主数据同步");
            StringBuilder errMsg = new StringBuilder();
            StringBuilder successMsg = new StringBuilder();
            DataTable mainedi = SQLHelper.ExecuteDataset(context.connStr, CommandType.Text, "SELECT COMPANY, DIST_CODE,INV_CODE, INV_NO, INV_DATE, INV_NAME, INV_TIN, DEPT_CODE, AMOUNT, TAX, PAY_DATE, [MONTH], [URL], SAP_DIST,[STATE] FROM MAIN_EDI_DATA ").Tables[0];
            DataRow dr;

            DirectoryInfo TheFolder = new DirectoryInfo(filePath);
            foreach (FileInfo NextFile in TheFolder.GetFiles("Delta_invoice_*.CSV"))
            {

                using (StreamReader sr = new StreamReader(NextFile.FullName, Encoding.UTF8))
                {
                    string str = sr.ReadToEnd();
                    string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < strlist.Length; i++)
                    {
                        string[] strs = strlist[i].Split('\t');
                        if (dic.ContainsValue(strs[2] + strs[3]))
                        {
                            errMsg.AppendLine(string.Format("文件:{4}-第{0}行供应商编码:{1}发票代码:{2}发票号码:{3}已经存在于MAIN_EDI_DATA表中", (i + 1), strs[1], strs[2], strs[3], NextFile.Name));
                            LogInfo.Log.Error(string.Format("文件:{4}-第{0}行供应商编码:{1}发票代码:{2}发票号码:{3}已经存在于MAIN_EDI_DATA表中", (i + 1), strs[1], strs[2], strs[3], NextFile.Name));
                            errorCount++;
                            continue;
                        }

                        dr = mainedi.NewRow();
                        dr["COMPANY"] = strs[0];
                        dr["DIST_CODE"] = strs[1];
                        dr["INV_CODE"] = strs[2];
                        dr["INV_NO"] = strs[3];
                        dr["INV_DATE"] = Convert.ToDateTime(strs[4]).ToString("yyyy-MM-dd");
                        dr["INV_NAME"] = strs[5];
                        dr["INV_TIN"] = strs[6];
                        dr["DEPT_CODE"] = strs[7];
                        dr["AMOUNT"] = Convert.ToDecimal(strs[8]).ToString();
                        dr["TAX"] = Convert.ToDecimal(strs[9]).ToString();
                        dr["PAY_DATE"] = Convert.ToDateTime(strs[10]).ToString("yyyy-MM-dd");
                        dr["MONTH"] = strs[11];
                        dr["URL"] = strs[12];
                        dr["SAP_DIST"] = strs[13];
                        dr["STATE"] = "0";

                        mainedi.Rows.Add(dr);
                        successMsg.AppendLine(string.Format("第{0}行供应商编码:{1}发票代码:{2}发票号码:{3}成功", i + 1, strs[1], strs[2], strs[3]));
                        successCount++;

                    }
                }

                FileMove(NextFile, folderPath_Queue);
            }
            try
            {
                 ExecuteMainData(mainedi, "MAIN_EDI_DATA");
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
