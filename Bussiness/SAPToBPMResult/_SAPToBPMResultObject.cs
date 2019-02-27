using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;

namespace SAPLinks.Bussiness.SAPToBPMResult
{
    public class _SAPToBPMResultObject
    {
        public BaseAction context { get; set; }
        //public string filePath { get; set; }
        //public string filter { get; set; }
        private string FilePath;
        public string filePath { get { return AddLastString(this.FilePath); } set { this.FilePath = value; } }
        private string FolderPath_Queue;
        public string folderPath_Queue { get { return AddLastString(this.FolderPath_Queue); } set { this.FolderPath_Queue = value; } }
        private string FolderPath_Success;
        public string folderPath_Success { get { return AddLastString(this.FolderPath_Success); } set { this.FolderPath_Success = value; } }
        private string FolderPath_Faild;
        public string folderPath_Faild { get { return AddLastString(this.FolderPath_Faild); } set { this.FolderPath_Faild = value; } }
        public SAPResultType sapType { get; set; }
        public Dictionary<string, string> main_Company_dic = new Dictionary<string, string>();
        public _SAPToBPMResultObject(BaseAction baseAction)
        {
            this.context = baseAction;
            LoadBaseData();
        }
        private void LoadBaseData()
        {
            DataTable dt = SQLHelper.ExecuteDataset(context.connStr, CommandType.Text, ("SELECT SAP_CD,CD FROM MAIN_COMPANY")).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
                main_Company_dic.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
        }
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
        /// <summary>
        /// 凭证错误回传获取发送邮件地址，如果不存在默认发送SAPLinks_MailAdress，否则发送SAPLinks_MailAdress_公司编码
        /// </summary>
        /// <param name="NextFile"></param>
        /// <returns></returns>
        protected string GetErrorMessageAddress(FileInfo NextFile)
        {
            string adress = string.Empty;
            try
            {
                string companyCode = NextFile.Name.Split('_')[1];
                adress = string.Format("SAPLinks_MailAdress_{0}", companyCode).ToAppSetting();
            }
            catch { }
            if (string.IsNullOrEmpty(adress))
                return "SAPLinks_MailAdress".ToAppSetting();
            else
                return adress;
        }
        /// <summary>
        /// 文件检查
        /// </summary>
        /// <param name="NextFile">errorlog、pay</param>
        /// <param name="fileNameHead"></param>
        /// <returns></returns>
        protected Boolean FileCheck(FileInfo NextFile, string fileNameHead)
        {
            string fileName = NextFile.Name.ToUpper().Replace(".TSV", "");//ERRORLOG_1020_20170926170100.TSV->ERRORLOG_1020_20170926170100
            string[] fileNameSplit = fileName.Split('_');
            if (fileNameSplit.Length != 3)
            {
                LogInfo.Log.Info(NextFile.FullName + "文件格式错误(ERR.TSV)");
                return false;
            }
            if (fileNameSplit[0] != fileNameHead)
            {
                LogInfo.Log.Info(NextFile.FullName + "文件格式错误(ERRORLOG)");
                return false;
            }
            DateTime errorDate = DateTime.Now;
            try
            {
                errorDate = DateTime.ParseExact(fileNameSplit[2], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            }
            catch
            {
                LogInfo.Log.Info(NextFile.FullName + "文件日期格式错误(yyyyMMddHHmmss)");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 文件执行移动及执行文件解析出的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="NextFile"></param>
        protected void Execute(string sql, FileInfo NextFile)
        {
            SqlCommand cmd = null;
            try
            {
                if (string.IsNullOrEmpty(sql))
                {
                    LogInfo.Log.Info(NextFile.FullName + "为空文件");
                    FileMove(NextFile, folderPath_Success);
                    return;
                }
                if (sapType == SAPResultType.Account)
                {
                    context.MessageQueue(GetErrorMessageAddress(NextFile), "SAPLinks_Title".ToAppSetting(), "SAPLinks_Message".ToAppSetting());
                    LogInfo.Log.Info("凭证回传文件:" + NextFile.FullName + "-" + NextFile.Name + "已发送邮件");
                }
                cmd = SQLHelper.GetTransactionSqlCommand(context.connStr);
                //数据插入
                SQLHelper.ExecuteNonQuery(ref cmd, sql);
                //文件移动
                FileMove(NextFile, folderPath_Success);
                //事物提交
                cmd.Transaction.Commit();
                cmd.Connection.Close();
                LogInfo.Log.Info("处理文件:【" + NextFile.FullName+ "】成功");
            }
            catch (Exception ex)
            {
                if (cmd != null)
                    cmd.Connection.Close();
                LogInfo.Log.Info("处理文件:【" + NextFile.FullName + "】失败,详细见错误日志");
                LogInfo.Log.Error(ex);
                FileMove(NextFile, folderPath_Faild);
                //发送邮件并移至错误文件夹中
                context.MessageQueue("SAP联携异常", NextFile.FullName + "文件处理异常：" + ex.Message);
            }
        }
        protected void Execute(DataTable dt, FileInfo NextFile)
        {
            if (dt.Rows.Count==0)
            {
                LogInfo.Log.Info(NextFile.FullName + "为空文件");
                FileMove(NextFile, folderPath_Success);
                return;
            }
            try
            {
                if (dt.PrimaryKey.Count() != 1)
                {
                    MSSQLFactory.BulkCopy(context.connStr, dt);
                    //文件移动
                    FileMove(NextFile, folderPath_Success);
                    LogInfo.Log.Info("处理文件:【" + NextFile.FullName + "】成功");
                }
                else
                {
                    //文件移动
                    FileMove(NextFile, folderPath_Faild);
                    LogInfo.Log.Info("处理文件:【" + NextFile.FullName + "】失败，主键有" + dt.PrimaryKey.Count() + "个");
                }
            }
            catch (Exception ex)
            {
                LogInfo.Log.Info("处理文件:【" + NextFile.FullName + "】失败,详细见错误日志");
                LogInfo.Log.Error(ex);
                FileMove(NextFile, folderPath_Faild);
                //发送邮件并移至错误文件夹中
                context.MessageQueue("SAP联携异常", NextFile.FullName + "文件处理异常：" + ex.Message);
            }
        }
        /// <summary>
        /// 数据插入
        /// </summary>
        /// <param name="sql"></param>
        protected bool Execute(string sql)
        {
            SqlCommand cmd = null;
            try
            {
                if (string.IsNullOrEmpty(sql))
                    return true;
                cmd = SQLHelper.GetTransactionSqlCommand(context.connStr);
                //数据插入
                SQLHelper.ExecuteNonQuery(ref cmd, sql);
                //事物提交
                cmd.Transaction.Commit();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (cmd != null)
                    cmd.Connection.Close();
                LogInfo.Log.Error(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 文件移动方法
        /// </summary>
        /// <param name="NextFile"></param>
        /// <param name="tagFolderPath"></param>
        protected void FileMove(FileInfo NextFile, string tagFolderPath)
        {
            LogInfo.Log.Info(string.Format("文件{0}移动至{1}", NextFile.Name, tagFolderPath + NextFile.Name));
            File.Move(NextFile.FullName, tagFolderPath + NextFile.Name);
        }
        /// <summary>
        /// 日期转换
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string SplitDate(string date)
        {
            string time = "";
            if (date.Length == 8)
            {
                string year = date.Substring(0, 4);
                string month = date.Substring(4, 2);
                string day = date.Substring(6, 2);
                time = year + "-" + month + "-" + day;
            }
            return time;
        }
    }
}
