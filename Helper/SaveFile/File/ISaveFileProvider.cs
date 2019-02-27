using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SAPLinks.Helper.SaveFile
{
    public abstract class ISaveFileProvider
    {
        protected SapSaveFileInfoCollection sapSaveFileInfos = new SapSaveFileInfoCollection();
        protected LinksQueueConnection linksQueueConnection;
        public ISaveFileProvider(LinksQueueConnection linksQueueConnection)
        {
            this.linksQueueConnection = linksQueueConnection;
        }
        public virtual void AddFileInfo(SapSaveFileInfo sapSaveFileInfo)
        {
            sapSaveFileInfos.Add(sapSaveFileInfo);
        }
        public virtual void RemoveFileInfo(SapSaveFileInfo sapSaveFileInfo)
        {
            sapSaveFileInfos.Remove(sapSaveFileInfo);
        }
        protected virtual bool Verification()
        {
            SapLinksQueueEntity sapLinksQueue = linksQueueConnection.GetSapLinksQueue();
            if (string.IsNullOrEmpty(sapLinksQueue.UpdateSql))
            {
                foreach (SapSaveFileInfo sapSaveFileInfo in sapSaveFileInfos)
                {
                    if (!string.IsNullOrEmpty(sapSaveFileInfo.SaveFileStream.ToString()))
                        return false;
                }
            }
            return true;
        }
        protected virtual bool FileIsNull()
        {
            if (sapSaveFileInfos.Count == 0)
                return false;
            return true;
        }
        protected virtual bool FolderExists()
        {
            foreach (SapSaveFileInfo sapSaveFileInfo in sapSaveFileInfos)
            {
                if (!MainFile.FolderExists(sapSaveFileInfo.SaveFilePath))
                    return false;
            }
            return true;
        }
        public abstract void DoSave();
        public void SaveFile()
        {
            if (!FileIsNull())
                throw new Exception("一个文件都没加，空文件都生成不出来");
            if (!FolderExists())
                throw new Exception("指定生成文件的路劲不存在");
            if (!Verification())
                throw new Exception("基础数据检测失败:SQL更新语句空，但文件内容不为空");
            this.DoSave();
        }
    }
}
