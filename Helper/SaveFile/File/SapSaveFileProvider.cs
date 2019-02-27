using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Helper.SaveFile
{
    public class SapSaveFileProvider : ISaveFileProvider
    {
        public SapSaveFileProvider(LinksQueueConnection linksQueueConnection) : base(linksQueueConnection)
        {
        }
        public override void DoSave()
        {
            linksQueueConnection.Open();
            linksQueueConnection.ExecuteQuery();
            try
            {
                foreach (SapSaveFileInfo sapSaveFileInfo in sapSaveFileInfos)
                {
                    MainFile.WriteFile(sapSaveFileInfo.SaveFilePath, sapSaveFileInfo.SaveFileName, sapSaveFileInfo.SaveFileStream.ToString(), sapSaveFileInfo.Encoding);
                }
                linksQueueConnection.Commit();
            }
            catch (Exception ex)
            {
                linksQueueConnection.Rollback();
                throw ex;
            }
        }
    }
}
