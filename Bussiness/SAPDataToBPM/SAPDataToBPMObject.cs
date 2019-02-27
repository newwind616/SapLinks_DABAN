using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SAPDataToBPM
{
    public abstract class SAPDataToBPMObject
    {
        protected BaseAction context { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath { get; set; }
        /// <summary>
        /// 删除公司过滤
        /// </summary>
        public string filter { get; set; }
        /// <summary>
        /// 可添加数据数量
        /// </summary>
        public int successCount = 0;
        /// <summary>
        /// 过滤数据数量
        /// </summary>
        public int errorCount = 0;
        /// <summary>
        /// MAIN_COMPANY公司信息，不在公司编码内信息过滤
        /// </summary>
        protected Dictionary<string, string> dic = new Dictionary<string, string>();
        #region 
        public SAPDataToBPMObject(string filePath, string filter, BaseAction baseAction, Center_Subject subject)
        {
            subject.LoadEvent += this.GetData;//加入处理事件
            this.filePath = filePath;
            this.filter = filter;
            this.context = baseAction;
            LoadBaseData();
        }
        public void LoadBaseData()
        {
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, CommandType.Text, ("SELECT SAP_CD,CD FROM MAIN_COMPANY")).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
                dic.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
        }
        public void ExecuteMainData(DataTable mainDatatable,string tableName)
        {
            using (System.Data.SqlClient.SqlBulkCopy bulk = new System.Data.SqlClient.SqlBulkCopy(context.connStr))
            {
                bulk.DestinationTableName = tableName;//设置目标表，这里是数据库中的student表
                for (int i = 0; i < mainDatatable.Columns.Count; i++)
                {
                    bulk.ColumnMappings.Add(i, i);
                }
                try
                {
                    bulk.WriteToServer(mainDatatable.GetChanges());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public string SplitDate(string date)
        {
            string time = "";
            if (date.Length == 8)
            {
                string year = date.Substring(0, 4);
                string month = date.Substring(4, 2);
                string day = date.Substring(6, 2);
                time = year + "-" + month + "-" + day;
            }
            return time;
        }
        public void Log()
        {
            Type type = this.GetType();
            LogInfo.Log.Info("" + type.FullName + "(" + filePath + ")【成功数据】：" + successCount + "条");
            LogInfo.Log.Info("" + type.FullName + "(" + filePath + ")【错误数据过滤】：" + errorCount + "条");

            
        }
        abstract public void GetData();
        #endregion
    }
}
