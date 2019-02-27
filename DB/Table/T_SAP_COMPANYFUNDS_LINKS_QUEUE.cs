using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks
{
    public class T_SAP_COMPANYFUNDS_LINKS_QUEUE
    {
        Dictionary<string,string> dic = new Dictionary< string,string>();
        public void Add(string applyNo)
        {
            if (!dic.ContainsKey(applyNo))
            {
                dic.Add(applyNo, applyNo);
            }
        }
        public Boolean Remove(string applyNo)
        {
            if (dic.ContainsKey(applyNo))
            {
                dic.Remove(applyNo);
                return true;
            }
            return false;
        }
        public string GetSql_ISLINK1()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> item in dic)
            {
                sb.AppendLine(ISLINK(item.Key, 1));
            }
            return sb.ToString();
        }
        public string GetSql_ISLINK0()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> item in dic)
            {
                sb.AppendLine(ISLINK(item.Key, 0));
            }
            return sb.ToString();
        }
        public string GetSql_DELETE()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> item in dic)
            {
                sb.AppendLine(string.Format("DELETE FROM BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_QUEUE WHERE APPLY_NO='{0}';", item.Key));
            }
            return sb.ToString();
        }
        private string ISLINK(string applyNo, int isLink)
        {
            /*****更新时间：20190123 内容：SAP_DATE=GetDate() *****/
            return string.Format("UPDATE BPMDB.DBO.SAP_COMPANYFUNDS_LINKS_QUEUE SET ISLINK={0},SAP_DATE=GetDate() WHERE APPLY_NO='{1}';", isLink, applyNo);
        }
    }
}
