using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SAPLinks.Bussiness
{
    public interface ISaveFileProvider
    {
        void SaveFile(SaveFileEntityCollection saveFileEntityCollection);
    }
    public abstract class SaveFileBaseProvider : ISaveFileProvider
    {
        public void SaveFile(SaveFileEntityCollection saveFileEntityCollection)
        {
            saveFileEntityCollection = CheckSaveFileEntityContextEncoding(saveFileEntityCollection);
            Verification(saveFileEntityCollection);
            DoSaveFile(saveFileEntityCollection);
        }
        /// <summary>
        /// 检查默认编码，如果未添加就添加UTF8格式
        /// </summary>
        /// <param name="saveFileEntityCollection"></param>
        /// <returns></returns>
        protected virtual SaveFileEntityCollection CheckSaveFileEntityContextEncoding(SaveFileEntityCollection saveFileEntityCollection)
        {
            for (int i = 0; i < saveFileEntityCollection.Count; i++)
            {
                if (saveFileEntityCollection[i].ContentEncoding == null)
                {
                    saveFileEntityCollection[i].ContentEncoding = new System.Text.UTF8Encoding(false);
                }
            }
            return saveFileEntityCollection;
        }
        protected virtual bool Verification(SaveFileEntityCollection saveFileEntityCollection)
        {
            string errMsg = string.Empty;
            foreach (SaveFileEntity saveFileEntity in saveFileEntityCollection)
            {
                PropertyInfo[] propertyInfos = saveFileEntity.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (propertyInfo.GetValue(saveFileEntity, null) == null)
                        throw new Exception("SaveFileEntity的字段:" + propertyInfo.Name + "为必填项！");
                }
            }
            return true;
        }
        public abstract void DoSaveFile(SaveFileEntityCollection saveFileEntityCollection);
    }
    public class SaveFileDBProvider : SaveFileBaseProvider
    {
        protected string ConnStr { get; set; }
        public SaveFileDBProvider(string connStr)
        {
            this.ConnStr = connStr;
        }
        public override void DoSaveFile(SaveFileEntityCollection saveFileEntityCollection)
        {
            if (string.IsNullOrEmpty(saveFileEntityCollection.UpdateLinkQueueSql))
            {
                foreach (SaveFileEntity saveFileEntity in saveFileEntityCollection)
                {
                    if (!SaveFile(saveFileEntity))
                    {
                        throw new Exception("文件" + saveFileEntity.FileFullName + "保存失败");
                    }
                }
            }
            else
            {
                SqlCommand cmd = SQLHelper.GetTransactionSqlCommand(ConnStr);
                SQLHelper.ExecuteNonQuery(ref cmd, saveFileEntityCollection.UpdateLinkQueueSql);
                foreach (SaveFileEntity saveFileEntity in saveFileEntityCollection)
                {
                    if (!SaveFile(saveFileEntity))
                    {
                        cmd.Transaction.Rollback();
                        throw new Exception("文件" + saveFileEntity.FileFullName + "保存失败");
                    }
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
        }
        #region 属性设置
        public SaveFileDBProvider SetConnStr(string connStr)
        {
            this.ConnStr = connStr;
            return this;
        } 
        #endregion
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="saveFileEntity"></param>
        /// <returns></returns>
        private bool SaveFile(SaveFileEntity saveFileEntity)
        {
            return MainFile.WriteFile(saveFileEntity.FilePath, saveFileEntity.FileFullName, saveFileEntity.Content, saveFileEntity.ContentEncoding);
        }
    }

    public class SaveFileEntity
    {
        /// <summary>
        /// 文件前缀名
        /// </summary>
        public string FileNamePrefix { get; set; }
        /// <summary>
        /// 文件后缀名
        /// </summary>
        public string FileNameMiddle { get; set; }
        /// <summary>
        /// 文件名中间段
        /// </summary>
        public string FileNameSuffix { get; set; }
        /// <summary>
        /// 文件全名
        /// </summary>
        public string FileFullName
        {
            get
            {
                return FileNamePrefix + FileNameMiddle + FileNameSuffix;
            }
        }
        private string filePath { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get { return SplitFilePath(filePath); } set { filePath = value; } }
        /// <summary>
        /// 文件内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 文件字符串编码
        /// </summary>
        public Encoding ContentEncoding { get; set; }

        private string SplitFilePath(string _filePath)
        {
            if (_filePath.LastIndexOf('\\') == _filePath.Length - 1)
                return _filePath;
            else
                return _filePath + "\\";
        }
        #region 属性设置
        public SaveFileEntity SetFileNamePrefix(string fileNamePrefix)
        {
            this.FileNamePrefix = fileNamePrefix;
            return this;
        }
        public SaveFileEntity SetFileNameMiddle(string fileNameMiddle)
        {
            this.FileNameMiddle = fileNameMiddle;
            return this;
        }
        public SaveFileEntity SetFileNameSuffix(string fileNameSuffix)
        {
            this.FileNameSuffix = fileNameSuffix;
            return this;
        }
        public SaveFileEntity SetFilePath(string filePath)
        {
            this.filePath = filePath;
            return this;
        }
        public SaveFileEntity SetContent(string content)
        {
            this.Content = content;
            return this;
        }
        public SaveFileEntity SetContentEncoding(Encoding contentEncoding)
        {
            this.ContentEncoding = contentEncoding;
            return this;
        }
        #endregion
    }
    public class SaveFileEntityCollection : List<SaveFileEntity>
    {
        public string UpdateLinkQueueSql { get; set; }
    }
}
