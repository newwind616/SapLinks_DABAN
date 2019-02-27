using SAPLinks.Helper.SaveFile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.KFSQ.DICS
{
    public class DICS_Action : BaseAction, ICompanyAction
    {
        string Code = "1020";
        T_SAP_COMPANYFUNDS_LINKS_QUEUE queue = new T_SAP_COMPANYFUNDS_LINKS_QUEUE();
        public void Start()
        {
            string filePath = "DICS_Path_KFSQ".ToAppSetting();
            string fileName = "DICS_Name_KFSQ".ToAppSetting();
            string fileExt = "DICS_Ext_KFSQ".ToAppSetting();
            LogInfo.Log.Info("DICS执行扩贩申请");
            StringBuilder fileData = GetData();
            SapLinksQueueEntity sapLinksQueue = new SapLinksQueueEntity("BPMDB".ToConnectionString(), queue.GetSql_ISLINK1());
            SAPLinks.Helper.SaveFile.ISaveFileProvider saveFileProvider = SapSaveFileProviderFactory.CreateSaveProvider(sapLinksQueue);
            fileName += string.Format("_{0}_{1}", Code, DateTime.Now.ToString("yyyyMMdd"));
            saveFileProvider.AddFileInfo(new SapSaveFileInfo(filePath, fileName + fileExt, fileData));
            saveFileProvider.SaveFile();
            LogInfo.Log.Info("DICS执行扩贩申请成功");
        }
        public StringBuilder GetData()
        {
            StringBuilder sb = new StringBuilder();
            string sql = @"select AA.APPLY_NO,AA.YS_NAME,AA.APPLY_DISPLAYNAME,AA.PAY_AMOUNT,AA.APPLY_DATE,AA.APPLY_DEPT from DABAN_BPM_DICS.dbo.YSJE_SQ_H AA
LEFT JOIN BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_QUEUE BB ON AA.APPLY_NO = BB.APPLY_NO
where BB.ISLINK = 0";
            DataTable dt = SQLHelper.ExecuteDataset(connStr, System.Data.CommandType.Text, sql).Tables[0];
            LogInfo.Log.Info("《扩贩申请》获取需处理数量：" + dt.Rows.Count + "条");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                AccVouch acc = new AccVouch();
                acc.I_KOKRS = "1000";//控制范围（I_KOKRS）
                acc.I_AUART = "ZZT1";//订单类型（I_AUART）
                acc.I_AUFNR = dt.Rows[i]["APPLY_NO"].ToString();//订单（I_AUFNR）
                acc.I_KTEXT = dt.Rows[i]["YS_NAME"].ToString();//描述（I_KTEXT）
                acc.I_BUKRS = Code;//公司代码（I_BUKRS）
                acc.I_SCOPE = "OC";//对象类（I_SCOPE）
                acc.I_USER0 = dt.Rows[i]["APPLY_DISPLAYNAME"].ToString();//申请人（I_USER0）
                acc.I_USER1 = "";//电话（I_USER1）
                acc.I_USER2 = "";//负责人（I_USER2）
                acc.I_USER3 = "";//电话1（I_USER3）
                acc.I_USER4 = dt.Rows[i]["PAY_AMOUNT"].ToString();//估算成本（I_USER4）
                acc.I_USER5 = Convert.ToDateTime(dt.Rows[i]["APPLY_DATE"]).ToString("yyyyMMdd");//申请日期（I_USER5）
                acc.I_USER6 = dt.Rows[i]["APPLY_DEPT"].ToString();//部门（I_USER6）
                queue.Add(dt.Rows[i]["APPLY_NO"].ToString());
                sb.AppendLine(acc.ToVouchString());
            }
            return sb;
        }
    }
}
