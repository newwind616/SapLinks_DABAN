using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SalesForceToDABAN
{
    public abstract class DataConvert
    {
        public SalesForceObject context { get; set; }

        public DataConvert(SalesForceObject action)
        {
            this.context = action;
        }

        /// <summary>
        /// 写入csv的内容
        /// </summary>
        public StringBuilder file_sb = new StringBuilder();

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
