using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public static class MergeObject
    {
        /// <summary>
        /// 2凭证集合合并
        /// </summary>
        /// <param name="srcListAccvouch">需要被合并的集合</param>
        /// <param name="tagListAccvouch">合并后的集合</param>
        /// <returns>返回合并后的集合tagListAccvouch</returns>
        public static List<AccVouch> MergeListAccVouch(this List<AccVouch> srcListAccvouch, List<AccVouch> tagListAccvouch)
        {
            if (srcListAccvouch == null)
                return tagListAccvouch;
            foreach (AccVouch item in srcListAccvouch)
            {
                tagListAccvouch.Add(item);
            }
            return tagListAccvouch;
        }
        /// <summary>
        /// 金额取绝对值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Abs(this string str)
        {
            try
            {
                return Math.Abs(Convert.ToDecimal(str)).ToString();
            }
            catch (Exception)
            {
                return str;
            }
        }
    }
}
