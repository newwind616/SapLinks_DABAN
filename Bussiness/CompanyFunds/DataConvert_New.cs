using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.CompanyFunds
{

    public abstract class DataConvert_New
    {

        public CompanyObject context { get; set; }
        public DataConvert_New(CompanyObject action)
        {
            this.context = action;
        }

        /// <summary>
        /// 写入TSV的联携内容
        /// </summary>
        public StringBuilder file_sb = new StringBuilder();
        /// <summary>
        /// 更新SAP_COMPANYFUNDS_LINKS_QUEUE联携状态
        /// </summary>
        public StringBuilder upLinks_sql = new StringBuilder();
        /// <summary>
        /// 是否有需要联携的数据
        /// </summary>
        public Boolean boo = false;

        protected string Create(params string[] fields)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in fields)
            {
                sb.Append(item + "\t");
            }
            return sb.ToString().Substring(0, sb.ToString().LastIndexOf("\t"));
        }
        protected string Create(List<string> fields)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in fields)
            {
                sb.Append(item + "\t");
            }
            return sb.ToString().Substring(0, sb.ToString().LastIndexOf("\t"));
        }
        public abstract void GetData();


    }
}
