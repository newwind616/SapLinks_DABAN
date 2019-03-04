using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace SAPLinks.Bussiness.EDIDataToDABAN
{
    public abstract class EDIDataToDABANObject //抽象类
    {
        protected BaseAction context { get; set; }

        public string filePath { get; set; }

        //public string filter { get; set; }

        public int successCount = 0;

        public int errorCount = 0;

        protected Dictionary<string, string> dic = new Dictionary<string, string>();

        public EDIDataToDABANObject(string filePath, BaseAction baseAction, Center_Subject subject)
        {
            subject.LoadEvent += this.GetData;//加入处理事件
            this.filePath = filePath;
          //  this.filter = filter;
            this.context = baseAction;
            LoadBaseData();
        }

        public void LoadBaseData()
        {
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, CommandType.Text, ("SELECT ID, INV_CODE,INV_NO FROM MAIN_EDI_DATA")).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
                dic.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString() + dt.Rows[i][2].ToString());
        }

        public void ExecuteMainData(DataTable mainDatatable, string tableName)
        {
            using (System.Data.SqlClient.SqlBulkCopy bulk = new System.Data.SqlClient.SqlBulkCopy(context.connStr))
            {
                bulk.DestinationTableName = tableName;//设置目标表
                for (int i = 0; i < mainDatatable.Columns.Count; i++)
                {

                    bulk.ColumnMappings.Add(mainDatatable.Columns[i].ColumnName, mainDatatable.Columns[i].ColumnName);
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

        public void Log()
        {
            Type type = this.GetType();
            LogInfo.Log.Info("" + type.FullName + "(" + filePath + ")【成功数据】：" + successCount + "条");
            LogInfo.Log.Info("" + type.FullName + "(" + filePath + ")【错误数据过滤】：" + errorCount + "条");


        }
        abstract public void GetData();
    }
}
