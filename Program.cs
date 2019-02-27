using System;
using System.Collections.Generic;
using System.Reflection;

namespace SAPLinks
{
    public delegate void TestActionHandler();
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(string.Format("当前版本：{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            //启动日志
            LogInfo.LoadLog4Net();
            //装载配置文件
            ConfigInfo.GetModules();
            LogInfo.Log.Info("--------------------------------程序启动--------------------------------");
            string paramet = string.Empty;
            if (args.Length != 0)
            {
                paramet = args[0];
                LogInfo.Log.Info("获得定时触发启动参数：" + paramet);
            }
            else
            {
                Console.WriteLine("请选择需要执行的功能：");
                foreach (KeyValuePair<string, Modules> item in ConfigInfo.FunDic)
                {
                    Console.WriteLine(item.Key + ":" + item.Value.Explain);
                }
                while (true)
                {
                    Console.WriteLine("输入数字序号：");
                    string outStr = Console.ReadLine();
                    if (ConfigInfo.FunDic.ContainsKey(outStr))
                        paramet = outStr;
                    if (!string.IsNullOrEmpty(paramet))
                    {
                        Console.WriteLine("程序执行期间请勿关闭，执行完毕后将自动关闭");
                        LogInfo.Log.Info("获得手动触发启动参数：" + paramet);
                        break;
                    }
                    Console.WriteLine("编号错误，请重新输入");
                }
            }
            if(!string.IsNullOrEmpty(paramet))
                new MainStartApp(paramet);
            LogInfo.Log.Info("--------------------------------程序结束--------------------------------"+ "\r\n");
        }
    }
}
