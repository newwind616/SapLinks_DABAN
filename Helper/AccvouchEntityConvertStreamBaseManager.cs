using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SAPLinks
{
    public delegate void AccvouchEntityConvertStreamBaseManagerModules();
    public class AccvouchEntityConvertStreamBaseManager
    {
        public AccvouchEntityConvertStreamBaseManagerModules accvouchEntityConvertStreamBaseManagerModules;
        protected Type type;
        protected StringBuilder resultStr = new StringBuilder();
        protected PropertyInfo[] propertyInfos;
        protected string customerAttributName = "ClassPropertySort";
        protected object classObject;
        /// <summary>
        /// 输出架构绑定，应该与当前实体分离，后续完善
        /// </summary>
        public AccvouchEntityConvertStreamBaseManager()
        {
            accvouchEntityConvertStreamBaseManagerModules += RemoveNoCustomAttributesProperty;
            accvouchEntityConvertStreamBaseManagerModules += OrderByIndex;
            accvouchEntityConvertStreamBaseManagerModules += OutPutVouchStr;
        }
        public void RemoveNoCustomAttributesProperty()
        {
            var list = propertyInfos.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                object[] obj = list[i].GetCustomAttributes(true);
                if (obj.Length != 1)
                {
                    list.RemoveAt(i);
                    continue;
                }
                Type type = obj[0].GetType();
                if (!type.Name.Equals(customerAttributName))
                {
                    list.RemoveAt(i);
                    continue;
                }
                if (!(list[i].GetCustomAttributes(true)[0] as ClassPropertySort).IsUsed)
                {
                    list.RemoveAt(i);
                    continue;
                }
            }
            propertyInfos = list.ToArray();
        }
        public void OrderByIndex()
        {
            for (int i = 0; i < propertyInfos.Length - 1; i++)
            {
                for (int j = i + 1; j < propertyInfos.Length; j++)
                {
                    if ((propertyInfos[i].GetCustomAttributes(false)[0] as ClassPropertySort).Index > (propertyInfos[j].GetCustomAttributes(false)[0] as ClassPropertySort).Index)
                    {
                        PropertyInfo temp = propertyInfos[i];
                        propertyInfos[i] = propertyInfos[j];
                        propertyInfos[j] = temp;
                    }
                }
            }
        }
        public void OutPutVouchStr()
        {
            int count = propertyInfos.Count();
            for (int i = 0; i < count; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                string propertyName = propertyInfo.Name;
                string value = Convert.ToString(propertyInfo.GetValue(classObject, null) == null ? "" : propertyInfo.GetValue(classObject, null));
                resultStr.Append(i == count - 1 ? value : value + "\t");
            }
        }
        public virtual string GetVouchString(object obj)
        {
            resultStr.Clear();
            this.classObject = obj;
            this.type = this.classObject.GetType();
            this.propertyInfos = this.type.GetProperties();
            accvouchEntityConvertStreamBaseManagerModules.Invoke();
            return resultStr.ToString();
        }
    }
}
