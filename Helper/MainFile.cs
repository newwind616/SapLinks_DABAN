using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SAPLinks
{
    public class MainFile
    {
        public static Boolean WriteFile(string filePath, string fileName, string msg)
        {
            return WriteFile(filePath, fileName, msg, new System.Text.UTF8Encoding(false));
        }

        public static Boolean WriteFile_(string filePath, string fileName, string msg)
        {
            return WriteFile(filePath, fileName, msg, new System.Text.UTF8Encoding(true));
        }
        public static Boolean WriteFile(string filePath, string fileName, string msg,Encoding encoding)
        {
            try
            {
                //写入前连接共享文件夹
                ConnectFile.connectState(filePath, "NisMail_UserName".ToAppSetting(), "NisMail_PWD".ToAppSetting());
                if (string.IsNullOrEmpty(msg))
                {
                    StreamWriter sw = File.CreateText(filePath + fileName);
                    sw.Close();
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(filePath + fileName, true,encoding))
                    {
                        if (fileName.Substring(fileName.IndexOf(".") + 1, 3) == "CSV")
                        {
                            sw.WriteLine(msg);
                        }
                        else
                        {
                            if (fileName.Substring(0, 3) != "JNL")//个人经费
                                msg = "\r\n" + msg;
                            if (!string.IsNullOrEmpty(msg))
                                msg = msg.Substring(0, msg.LastIndexOf("\r\n"));
                            if (fileName.Substring(0, 3) != "JNL")
                            {
                                sw.WriteLine(msg);//公司经费开始写入值
                            }
                            else
                            {
                                sw.WriteLine(msg);//个人经费开始写入值
                            }
                        }
                       
                    }
                }
                LogInfo.Log.Info("生成文件：" + filePath + fileName);
                return true;
            }
            catch (Exception ex)
            {
                LogInfo.Log.Error(ex);
                throw ex;
            }
        }
        public static Boolean FolderExists(string folderPath)
        {
            return Directory.Exists(folderPath);
        }
    }
}
