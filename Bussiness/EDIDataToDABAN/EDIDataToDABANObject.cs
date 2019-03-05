using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;


namespace SAPLinks.Bussiness.EDIDataToDABAN
{
    public abstract class EDIDataToDABANObject //抽象类
    {
        protected BaseAction context { get; set; }

        public string filePath { get; set; }

        //public string filter { get; set; }

        public int successCount = 0;

        public int errorCount = 0;

        private string FolderPath_Queue;

        public string folderPath_Queue {
            get { return AddLastString(this.FolderPath_Queue); } set { this.FolderPath_Queue = value; }
        }

        protected Dictionary<string, string> dic = new Dictionary<string, string>();

        /// <summary>
        /// 检查文件路径末尾是否带\如果没带补\
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string AddLastString(string str)
        {
            if (str.LastIndexOf('\\') == str.Length - 1)
                return str;
            else
                return str + "\\";
        }



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

        /// <summary>
        /// 文件移动方法
        /// </summary>
        /// <param name="NextFile"></param>
        /// <param name="tagFolderPath"></param>
        protected void FileMove(FileInfo NextFile, string tagFolderPath)
        {
            LogInfo.Log.Info(string.Format("文件:{0}移动至{1}", NextFile.Name, tagFolderPath + NextFile.Name));
            File.Move(NextFile.FullName, tagFolderPath + NextFile.Name);
        }
        abstract public void GetData();
    }
}
