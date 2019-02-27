using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Config;
using log4net;
using System.Reflection;
using System.IO;

namespace SAPLinks
{
    public class LogInfo
    {
        public static ILog Log = null;
        public static void LoadLog4Net()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log4net.config";
            FileInfo fileInfo = new FileInfo(filePath);
            XmlConfigurator.ConfigureAndWatch(fileInfo);
            Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
