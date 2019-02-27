using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPDataToBPM.SAP1
{
    public class MAIN_CUSTOMER : SAPDataToBPMObject
    {
        public MAIN_CUSTOMER(string paths, string filters, BaseAction baseAction, Center_Subject subject) : base(paths, filters, baseAction, subject)
        {
        }

        public override void GetData()
        {
            LogInfo.Log.Info("执行SAP1客户行主数据同步");
            StringBuilder errMsg = new StringBuilder();
            StringBuilder successMsg = new StringBuilder();
            SQLHelper.ExecuteNonQuery(context.connStr, "DELETE FROM MAIN_CUSTOMER WHERE COMPANY IN(" + filter + ")");
            DataTable mainsupplier = SQLHelper.ExecuteDataset(context.connStr, CommandType.Text, "SELECT COMPANY, CSR_ID, CSR_NAME, CSR_ABB,FINCODE, DEL_FLG, KNA1KUNNR, KNA1LAND1, KNA1NAME1, KNA1NAME2, KNA1SORTL, KNA1ADRNR, KNA1ERDAT, KNA1ERNAM, KNA1KONZS, KNA1KTOKD, KNA1LIFNR, KNA1LOEVM, KNA1SPERR, KNA1SPRAS, KNA1STCD1, KNA1VBUND, KNA1STCEG, KNA1ZZDESTINATION, KNA1ZZPREFECTURE, KNA1ZZCITY, KNA1ZZAREACODE, KNA1ZZBIZMINUNIT, KNA1ZZCONCLASS, KNA1ZZRACUSTTYPE, KNA1ZZDEVDATE, KNA1ZZGRINOUTFLG, KNA1ZZVKBUR, KNA1ZKAIPIAO, KNA1ZHUISHOU, KNA1ZZGSQSEC_CUST, ADRCDATE_FROM, ADRCNATION, ADRCDATE_TO, ADRCTITLE, ADRCNAME1, ADRCNAME2, ADRCNAME3,ADRCNAME4, ADRCNAME_CO, ADRCCITY1, ADRCPOST_CODE1, ADRCPO_BOX, ADRCTRANSPZONE, ADRCSTREET, ADRCHOUSE_NUM1, ADRCSTR_SUPPL1,ADRCSTR_SUPPL2, ADRCSTR_SUPPL3, ADRCLOCATION, ADRCBUILDING, ADRCFLOOR, ADRCROOMNUMBER, ADRCCOUNTRY, ADRCLANGU, ADRCREGION, ADRCSORT1, ADRCSORT2, ADRCTEL_NUMBER, ADRCFAX_NUMBER, KNB1BUKRS, KNB1PERNR, KNB1ERDAT, KNB1ERNAM, KNB1SPERR, KNB1LOEVM,KNB1AKONT, KNB1ZTERM, KNVVVKORG, KNVVVTWEG, KNVVSPART, KNVVERNAM, KNVVERDAT, KNVVLOEVM, KNVVKALKS, KNVVKONDA, KNVVINCO1, KNVVINCO2, KNVVVSBED, KNVVWAERS, KNVVZTERM, KNVVVWERK, KNBKBANKS, KNBKBANKL, KNBKBANKN, KNBKBKONT, KNBKBKREF, KNBKKOINH, KNVKNAMEV, KNVKNAME1, KNVKTELF1, BNKABANKA FROM MAIN_CUSTOMER WHERE COMPANY IN(" + filter + ")").Tables[0];
            //向表中添加数据
            DataRow dr;
            ConnectFile.connectState(filePath, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
            {
                string str = sr.ReadToEnd();
                string[] strlist = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < strlist.Length; i++)
                {
                    string[] strs = strlist[i].Split('\t');
                    if (strs.Length < 92)
                    {
                        errMsg.AppendLine("第" + (i + 1) + "行数据完整性异常");
                        errorCount++;
                        continue;
                    }
                    if (!dic.ContainsKey(strs[59]))
                    {
                        errMsg.AppendLine(string.Format("第{0}行公司编码:{1}客户行编码:{2}客户行名称:{3}不存在于MAIN_COMPANY表中", (i + 1), strs[59], strs[0], strs[2]));
                        errorCount++;
                        continue;
                    }
                    //string key = dic[strs[47]] + strs[0];
                    dr = mainsupplier.NewRow();
                    dr["COMPANY"] = dic[strs[59]];
                    dr["CSR_ID"] = strs[0];
                    dr["CSR_NAME"] = strs[2];
                    dr["CSR_ABB"] = strs[4];
                    dr["FINCODE"] = strs[65];
                    dr["DEL_FLG"] = strs[63].ToUpper() == "X" ? 1 : 0;
                    dr["KNA1KUNNR"] = strs[0];
                    dr["KNA1LAND1"] = strs[1];
                    dr["KNA1NAME1"] = strs[2];
                    dr["KNA1NAME2"] = strs[3];
                    dr["KNA1SORTL"] = strs[4];
                    dr["KNA1ADRNR"] = strs[5];
                    if (strs[6] != "")
                    {
                        if (strs[6].Length == 8)
                        {
                            dr["KNA1ERDAT"] = SplitDate(strs[6]);//date
                        }
                        else
                        {
                            dr["KNA1ERDAT"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        dr["KNA1ERDAT"] = DBNull.Value;
                    }
                    dr["KNA1ERNAM"] = strs[7];
                    dr["KNA1KONZS"] = strs[8];
                    dr["KNA1KTOKD"] = strs[9];
                    dr["KNA1LIFNR"] = strs[10];
                    dr["KNA1LOEVM"] = strs[11];
                    dr["KNA1SPERR"] = strs[12];
                    dr["KNA1SPRAS"] = strs[13];
                    dr["KNA1STCD1"] = strs[14];
                    dr["KNA1VBUND"] = strs[15];
                    dr["KNA1STCEG"] = strs[16];
                    dr["KNA1ZZDESTINATION"] = strs[17];
                    dr["KNA1ZZPREFECTURE"] = strs[18];
                    dr["KNA1ZZCITY"] = strs[19];
                    dr["KNA1ZZAREACODE"] = strs[20];
                    dr["KNA1ZZBIZMINUNIT"] = strs[21];
                    dr["KNA1ZZCONCLASS"] = strs[22];
                    dr["KNA1ZZRACUSTTYPE"] = strs[23];
                    dr["KNA1ZZDEVDATE"] = strs[24];
                    dr["KNA1ZZGRINOUTFLG"] = strs[25];
                    dr["KNA1ZZVKBUR"] = strs[26];
                    dr["KNA1ZKAIPIAO"] = strs[27];
                    dr["KNA1ZHUISHOU"] = strs[28];
                    dr["KNA1ZZGSQSEC_CUST"] = strs[29];
                    if (strs[30] != "")
                    {
                        if (strs[30].Length == 8)
                        {
                            dr["ADRCDATE_FROM"] = SplitDate(strs[30]);//date
                        }
                        else
                        {
                            dr["ADRCDATE_FROM"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        dr["ADRCDATE_FROM"] = DBNull.Value;
                    }
                    dr["ADRCNATION"] = strs[31];
                    if (strs[32] != "")
                    {
                        if (strs[32].Length == 8)
                        {
                            dr["ADRCDATE_TO"] = SplitDate(strs[32]);//date
                        }
                        else
                        {
                            dr["ADRCDATE_TO"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        dr["ADRCDATE_TO"] = DBNull.Value;
                    }
                    dr["ADRCTITLE"] = strs[33];
                    dr["ADRCNAME1"] = strs[34];
                    dr["ADRCNAME2"] = strs[35];
                    dr["ADRCNAME3"] = strs[36];
                    dr["ADRCNAME4"] = strs[37];
                    dr["ADRCNAME_CO"] = strs[38];
                    dr["ADRCCITY1"] = strs[39];
                    dr["ADRCPOST_CODE1"] = strs[40];
                    dr["ADRCPO_BOX"] = strs[41];
                    dr["ADRCTRANSPZONE"] = strs[42];
                    dr["ADRCSTREET"] = strs[43];
                    dr["ADRCHOUSE_NUM1"] = strs[44];
                    dr["ADRCSTR_SUPPL1"] = strs[45];
                    dr["ADRCSTR_SUPPL2"] = strs[46];
                    dr["ADRCSTR_SUPPL3"] = strs[47];
                    dr["ADRCLOCATION"] = strs[48];
                    dr["ADRCBUILDING"] = strs[49];
                    dr["ADRCFLOOR"] = strs[50];
                    dr["ADRCROOMNUMBER"] = strs[51];
                    dr["ADRCCOUNTRY"] = strs[52];
                    dr["ADRCLANGU"] = strs[53];
                    dr["ADRCREGION"] = strs[54];
                    dr["ADRCSORT1"] = strs[55];
                    dr["ADRCSORT2"] = strs[56];
                    dr["ADRCTEL_NUMBER"] = strs[57];
                    dr["ADRCFAX_NUMBER"] = strs[58];
                    dr["KNB1BUKRS"] = strs[59];//COMPANY
                    dr["KNB1PERNR"] = strs[60];
                    if (strs[61] != "")
                    {
                        if (strs[61].Length == 8)
                        {
                            dr["KNB1ERDAT"] = SplitDate(strs[61]);//date
                        }
                        else
                        {
                            dr["KNB1ERDAT"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        dr["KNB1ERDAT"] = DBNull.Value;
                    }
                    dr["KNB1ERNAM"] = strs[62];
                    dr["KNB1SPERR"] = strs[63];
                    dr["KNB1LOEVM"] = strs[64];
                    dr["KNB1AKONT"] = strs[65];
                    dr["KNB1ZTERM"] = strs[66];
                    dr["KNVVVKORG"] = strs[67];
                    dr["KNVVVTWEG"] = strs[68];
                    dr["KNVVSPART"] = strs[69];
                    dr["KNVVERNAM"] = strs[70];
                    if (strs[71] != "")
                    {
                        if (strs[71].Length == 8 && strs[71] != "00000000")
                        {
                            dr["KNVVERDAT"] = SplitDate(strs[71]);//date
                        }
                        else
                        {
                            dr["KNVVERDAT"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        dr["KNVVERDAT"] = DBNull.Value;
                    }
                    dr["KNVVLOEVM"] = strs[72];
                    dr["KNVVKALKS"] = strs[73];
                    dr["KNVVKONDA"] = strs[74];
                    dr["KNVVINCO1"] = strs[75];
                    dr["KNVVINCO2"] = strs[76];
                    dr["KNVVVSBED"] = strs[77];
                    dr["KNVVWAERS"] = strs[78];
                    dr["KNVVZTERM"] = strs[79];
                    dr["KNVVVWERK"] = strs[80];
                    dr["KNBKBANKS"] = strs[81];
                    dr["KNBKBANKL"] = strs[82];
                    dr["KNBKBANKN"] = strs[83];
                    dr["KNBKBKONT"] = strs[84];
                    dr["KNBKBKREF"] = strs[85];
                    dr["KNBKKOINH"] = strs[86];
                    dr["KNVKNAMEV"] = strs[87];
                    dr["KNVKNAME1"] = strs[88];
                    dr["KNVKTELF1"] = strs[89];
                    dr["BNKABANKA"] = strs[90];//97 个字段
                    mainsupplier.Rows.Add(dr);
                    successMsg.AppendLine(string.Format("第{0}行公司:{1}客户行编码:{2}客户行名称:{3}成功", i + 1, dic[strs[59]], strs[0], strs[2]));
                    successCount++;
                }
            }
            try
            {
                ExecuteMainData(mainsupplier, "MAIN_CUSTOMER");
            }
            catch (Exception ex)
            {
                //LogInfo.Log.Error(ex);
                //exception = ex;
                throw ex;
            }
            Log();
        }

        public string SplitDate(string date)
        {
            string time = "";
            if (date.Length == 8)
            {
                string year = date.Substring(0, 4);
                string month = date.Substring(4, 2);
                string day = date.Substring(6, 2);
                time = year + "-" + month + "-" + day;
            }

            return time;
        }

    }
}
