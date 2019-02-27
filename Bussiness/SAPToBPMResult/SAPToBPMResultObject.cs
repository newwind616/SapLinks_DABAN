using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult
{
    public abstract class SAPToBPMResultObject : _SAPToBPMResultObject
    {
        public SAPToBPMResultObject(BaseAction baseAction, Center_Subject subject) : base(baseAction)
        {
            subject.LoadEvent += this.GetData;//加入处理事件
        }
        protected virtual string[] XBLNRPrefixSuffix(int xblnrIndex, string[] strs)
        {
            return new string[2] { strs[xblnrIndex].Substring(0, 12), strs[0].Substring(12, 4) };
        }
        abstract public void GetData();
    }
}
