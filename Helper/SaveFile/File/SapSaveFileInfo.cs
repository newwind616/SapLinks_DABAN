using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Helper.SaveFile
{
    public class SapSaveFileInfo
    {
        private string savefilePath { get; set; }
        public string SaveFilePath
        {
            get
            {
                if (savefilePath.LastIndexOf('\\') == savefilePath.Length - 1)
                    return savefilePath;
                else
                    return savefilePath + "\\";
            }
            set { this.savefilePath = value; }
        }
        public string SaveFileName { get; set; }
        public StringBuilder SaveFileStream { get; set; }
        public Encoding Encoding { get; set; }
        /// <summary>
        /// 实例化文件信息，带字符编码格式
        /// </summary>
        /// <param name="savefilePath">文件保存路径</param>
        /// <param name="saveFileName">文件名称</param>
        /// <param name="saveFileStream">文件内容</param>
        /// <param name="encoding">字符编码</param>
        public SapSaveFileInfo(string savefilePath, string saveFileName, StringBuilder saveFileStream, Encoding encoding)
        {
            this.savefilePath = savefilePath;
            this.SaveFileName = saveFileName;
            this.SaveFileStream = saveFileStream;
            this.Encoding = encoding;
        }
        /// <summary>
        /// 实例化文件信息，默认字符编码：new System.Text.UTF8Encoding(false)
        /// </summary>
        /// <param name="savefilePath">文件保存路径</param>
        /// <param name="saveFileName">文件名称</param>
        /// <param name="saveFileStream">文件内容</param>
        public SapSaveFileInfo(string savefilePath, string saveFileName, StringBuilder saveFileStream)
        {
            this.savefilePath = savefilePath;
            this.SaveFileName = saveFileName;
            this.SaveFileStream = saveFileStream;
            this.Encoding = new System.Text.UTF8Encoding(false);
        }
    }
    public class SapSaveFileInfoCollection : List<SapSaveFileInfo> { }
}
