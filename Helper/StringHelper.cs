using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks
{
    public static class StringHelper
    {
        /// <summary>
        /// 格式化编号
        /// 转换K201010230001-01成K201010230001
        /// BB201700321-1
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        public static string ToFormatApply_No(this string apply_no)
        {
            if (apply_no.LastIndexOf('-') == -1)
                return apply_no;
            else
                return apply_no.Substring(0, apply_no.LastIndexOf('-'));
        }
        public static string ToTSVString(this List<string> fields)
        {
            StringBuilder sb = new StringBuilder();
            if (fields.Count == 0)
                return sb.ToString();
            foreach (string item in fields)
            {
                sb.Append(item + "\t");
            }
            return sb.ToString().Substring(0, sb.ToString().LastIndexOf("\t"));
        }
        /// <summary>
        /// 截取指定长度字符串，从下标0开始，如果不满足就返回全部
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string SubstringExt(this string str, int len)
        {
            if (str.Length > len)
                str = str.Substring(0, len);
            return str;
        }
    }
}
