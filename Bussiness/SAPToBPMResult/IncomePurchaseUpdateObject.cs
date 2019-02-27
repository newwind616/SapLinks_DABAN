using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPToBPMResult
{
    public abstract class IncomePurchaseUpdateObject : SAPToBPMResultObject
    {
        public IncomePurchaseUpdateObject(BaseAction baseAction, Center_Subject subject) : base(baseAction, subject)
        {
        }
        protected AIncomePurchaseUpdate aIncomePurchaseUpdate;
        /// <summary>
        /// 是否执行主数据更新
        /// true:更新
        /// false:不更新
        /// </summary>
        protected bool IsExecuteQuery { get { return true; } }
    }
}
