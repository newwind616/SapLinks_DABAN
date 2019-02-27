using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public interface IFKTZSMainStartApp
    {
        /// <summary>
        /// 根据基础信息装载凭证数据
        /// </summary>
        /// <param name="applyNoBasicEntity"></param>
        /// <returns></returns>
       List<AccVouch> Load(ApplyNoBasicEntity applyNoBasicEntity);
        /// <summary>
        /// 获取队列applyNo集合
        /// </summary>
        /// <returns></returns>
        ApplyNoEntityCollection GetApplyNoEntitys();
    }
}
