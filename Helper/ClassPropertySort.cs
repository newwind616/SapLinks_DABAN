using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ClassPropertySort : Attribute
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }
        public bool IsUsed { get; set; }
    }
}
