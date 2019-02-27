using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SAPLinks
{
    public static class ConfigInfo
    {
        public static string ToAppSetting(this string appName)
        {
            try
            {
                string path = Path.GetDirectoryName(typeof(ConfigInfo).Assembly.Location)+"\\App.config";
                ExeConfigurationFileMap file = new ExeConfigurationFileMap();
                file.ExeConfigFilename = path;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
                AppSettingsSection appsection = (AppSettingsSection)config.GetSection("appSettings");
                return appsection.Settings[appName].Value;
            }
            catch (Exception ex)
            {
                LogInfo.Log.Error("【" + appName + "】" + ex.Message);
                throw ex;
            }
        }

        public static string ToConnectionString(this string connName)
        {
            try
            {
                string path = Path.GetDirectoryName(typeof(ConfigInfo).Assembly.Location) + "\\App.config";
                ExeConfigurationFileMap file = new ExeConfigurationFileMap();
                file.ExeConfigFilename = path;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
                ConnectionStringsSection appsection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                return appsection.ConnectionStrings[connName].ConnectionString;
            }
            catch (Exception ex)
            {
                LogInfo.Log.Error(ex);
                throw ex;
            }
        }

        public static Dictionary<string, Modules> FunDic = new Dictionary<string, Modules>();
        public static void GetModules()
        {
            try
            {
                FunDic.Clear();
                XDocument doc = XDocument.Load(Path.GetDirectoryName(typeof(ConfigInfo).Assembly.Location) + "\\module.xml");
                XElement root = doc.Root;
                foreach (XElement element in root.Elements("item"))
                {
                    Modules head = new Modules();
                    head.Index = element.Attribute("Index").Value;
                    head.Caption = element.Attribute("Caption").Value;
                    head.Explain = element.Attribute("Explain").Value;
                    head.FormPath = element.Attribute("FormPath").Value;
                    FunDic.Add(head.Index,head);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("非开发人员请勿修改直接修改menu.xml"+ ex.Message);
                throw ex;
            }
        }
    }
    public class Modules
    {
        public string Index { get; set; }
        public string Caption { get; set; }
        public string FormPath { get; set; }
        public string Explain { get; set; }
    }

}
