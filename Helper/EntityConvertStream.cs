using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks
{
    public static class EntityConvertStream
    {
        public static string ToVouchString(this object obj)
        {
            AccvouchEntityConvertStreamBaseManager accvouchEntityConvertStreamBaseManager = new AccvouchEntityConvertStreamBaseManager();
            return accvouchEntityConvertStreamBaseManager.GetVouchString(obj);
        }
        public static string ToVouchString(this List<string> fields)
        {
            StringBuilder sb = new StringBuilder();
            if (fields.Count == 0)
                return sb.ToString();
            foreach (string item in fields)
            {
                sb.Append(item + "\t");
            }
            fields.Clear();
            return sb.ToString().Substring(0, sb.ToString().LastIndexOf("\t"));
        }
    }
}
