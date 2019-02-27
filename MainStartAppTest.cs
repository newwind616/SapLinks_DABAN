using SAPLinks.Bussiness;
using SAPLinks.Helper.SaveFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks
{
    public class MainStartAppTest : BaseAction, ICompanyAction
    {
        public void Start()
        {
            LogInfo.Log.Info("执行测试代码");
            Go();
            LogInfo.Log.Info("执行测试代码完成");
        }
        public void Go()
        {
            SapLinksQueueEntity sapLinksQueue = new SapLinksQueueEntity("BPMDB".ToConnectionString(), "update BPMSysUsers set Password='sa' where Account='asdasdasdasdads'");
            SAPLinks.Helper.SaveFile.ISaveFileProvider saveFileProvider = SapSaveFileProviderFactory.CreateSaveProvider(sapLinksQueue);
            saveFileProvider.AddFileInfo(new SapSaveFileInfo("D:\\", "JJ.txt", new StringBuilder().AppendLine("奥术大师大三大所多")));
            saveFileProvider.SaveFile();
        }
    }
}
