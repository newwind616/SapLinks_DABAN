using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult.SECTION
{
    public class SECTION : SAPToBPMResultObject
    {
        public SECTION(string folderPath_Queue, string folderPath_Success, string folderPath_Faild, BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
            this.folderPath_Queue = folderPath_Queue;
            this.folderPath_Success = folderPath_Success;
            this.folderPath_Faild = folderPath_Faild;
            //this.sapType = SAPResultType.Pay;
        }

        public override void GetData()
        {
            ConnectFile.connectState(folderPath_Queue, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            DirectoryInfo TheFolder = new DirectoryInfo(folderPath_Queue);
            foreach (FileInfo NextFile in TheFolder.GetFiles("*_SECTION_?_????????.TSV"))//*_SECTION_?_????????.TSV  //DSC_SECTION_C_20180202
            {
                string sql = AggData(NextFile);
                Execute(sql, NextFile);
            }
        }
        private string AggData(FileInfo NextFile)
        {
            string str = string.Empty;
            using (StreamReader sr = new StreamReader(NextFile.FullName, Encoding.Default))
            {
                str = sr.ReadToEnd();
            }
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string h_a_b_c = "";//主表或明细表【区分主表/从表】
            string tablename = "DABAN_BPM_";//数据库名称【解析文件名称不同公司存入不同数据库】
            string[] filenamelist = NextFile.Name.Split('_');
            if(filenamelist.Length==4)
            {
                tablename += filenamelist[0];//DABAN_BPM_DICG
                h_a_b_c = filenamelist[2];
            }
            for (int i = 0; i < strlist.Length; i++)
            {
                //拼接sql
                string[] strs = strlist[i].Split('\t');
                if (h_a_b_c == "H")
                {
                    //主表
                    string area_h = strs[0];
                    string contractno_h = strs[1];
                    string customername_h = strs[2];
                    string projectcontent_h = strs[3];
                    sb.AppendLine(string.Format("INSERT INTO [{0}].[dbo].[MAIN_SECTION_H](AREA,CONTRACTNO,CUSTOMERNAME,PROJECTCONTENT) VALUES ('{1}','{2}','{3}','{4}');", tablename, area_h, contractno_h, customername_h, projectcontent_h));
                }
                if (h_a_b_c == "A")
                {
                    //从表1
                    string contractno_A = strs[0];
                    string projectname_A = strs[1];
                    Decimal dabanhsj_A = Convert.ToDecimal(strs[2] == "" ? "0" : strs[2]);
                    Decimal dabanbhsj_A = Convert.ToDecimal(strs[3] == "" ? "0" : strs[3]);
                    Decimal snsbgydhsj_A = Convert.ToDecimal(strs[4] == "" ? "0" : strs[4]);
                    Decimal snsbgydbhsj_A = Convert.ToDecimal(strs[5] == "" ? "0" : strs[5]);
                    Decimal wbgsdhsj_A = Convert.ToDecimal(strs[6] == "" ? "0" : strs[6]);
                    Decimal wbgsdbhsj_A = Convert.ToDecimal(strs[7] == "" ? "0" : strs[7]);
                    Decimal cl_A = Convert.ToDecimal(strs[8] == "" ? "0" : strs[8]);
                    Decimal cll_A = Convert.ToDecimal(strs[9] == "" ? "0" : strs[9]);
                    string sgbpbh_A = strs[10];
                    Decimal bpckj_A = Convert.ToDecimal(strs[11] == "" ? "0" : strs[11]);
                    sb.AppendLine(string.Format("INSERT INTO [{0}].[dbo].[MAIN_SECTION_A](CONTRACTNO,PROJECTNAME,DABANHSJ,DABANBHSJ,SNSBGYDHSJ,SNSBGYDBHSJ,WBGSDHSJ,WBGSDBHSJ,CL,CLL,SGBPBH,BPCKJ) VALUES ('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},'{11}',{12});", tablename, contractno_A, projectname_A, dabanhsj_A, dabanbhsj_A, snsbgydhsj_A, snsbgydbhsj_A, wbgsdhsj_A, wbgsdbhsj_A, cl_A, cll_A, sgbpbh_A, bpckj_A));
                }
                if (h_a_b_c == "B")
                {
                    //从表2
                    string contractno_B = strs[0];
                    string zybgsphyd_B = strs[1];
                    Decimal dabanhsj_B = Convert.ToDecimal(strs[2] == "" ? "0" : strs[2]);
                    Decimal dabanbhsj_B = Convert.ToDecimal(strs[3] == "" ? "0" : strs[3]);
                    Decimal snsbgydhsj_B = Convert.ToDecimal(strs[4] == "" ? "0" : strs[4]);
                    Decimal snsbgydbhsj_B = Convert.ToDecimal(strs[5] == "" ? "0" : strs[5]);
                    Decimal wbgsdhsj_B = Convert.ToDecimal(strs[6] == "" ? "0" : strs[6]);
                    Decimal wbgsdbhsj_B = Convert.ToDecimal(strs[7] == "" ? "0" : strs[7]);
                    Decimal cl_B = Convert.ToDecimal(strs[8] == "" ? "0" : strs[8]);
                    Decimal cll_B = Convert.ToDecimal(strs[9] == "" ? "0" : strs[9]);
                    sb.AppendLine(string.Format("INSERT INTO [{0}].[dbo].[MAIN_SECTION_B](CONTRACTNO,ZYBGSBHYD,DABANHSJ,DABANBHSJ,SNSBGYDHSJ,SNSBGYDBHSJ,WBGSDHSJ,WBGSDBHSJ,CL,CLL) VALUES ('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10});", tablename, contractno_B, zybgsphyd_B, dabanhsj_B, dabanbhsj_B, snsbgydhsj_B, snsbgydbhsj_B, wbgsdhsj_B, wbgsdbhsj_B, cl_B, cll_B));
                }
                if (h_a_b_c == "C")
                {
                    //从表3
                    string contractno_C = strs[0];
                    string equipmenttype_C = strs[1];
                    string jobcontent_C = strs[2]+" "+strs[3]; ;
                    int byds_C = Convert.ToInt32(strs[4]);
                    int bycs_C = Convert.ToInt32(strs[5]);
                    int equipmentnum_C = Convert.ToInt32(strs[6]);
                    Decimal price_C = Convert.ToDecimal(strs[7] == "" ? "0" : strs[7]);
                    Decimal total_C = Convert.ToDecimal(strs[8] == "" ? "0" : strs[8]);
                    sb.AppendLine(string.Format("INSERT INTO [{0}].[dbo].[MAIN_SECTION_C](CONTRACTNO,EQUIPMENTTYPE,JOBCONTENT,BYDS,BYCS,EQUIPMENTNUM,PRICE,TOTAL) VALUES ('{1}','{2}','{3}',{4},{5},{6},{7},{8});", tablename, contractno_C, equipmenttype_C, jobcontent_C, byds_C, bycs_C, equipmentnum_C, price_C, total_C));
                }
            }
            return sb.ToString();
        }
    }
}
