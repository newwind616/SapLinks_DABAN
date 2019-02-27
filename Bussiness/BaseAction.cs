using System;
using System.Text;
using System.Data;
using System.Reflection;

namespace SAPLinks.Bussiness
{
    public class BaseAction:EventArgs,IBaseAction
    {
        public string errMsg = string.Empty;
        public string ErrMsg()
        {
            return errMsg;
        }
        public string connStr
        {
            get { return "BPMDB".ToConnectionString(); }
        }
        public Boolean PostEvent(string methodName)
        {
            try
            {
                Type type = this.GetType();
                MethodInfo method = type.GetMethod(methodName);
                if (method == null)
                    throw new Exception("Then Method 【" + methodName + "】 not implemented!");
                method.Invoke(this, null);
                return true;
            }
            catch (Exception ex)
            {
                errMsg = (ex.InnerException.Message + "\r\n" + ex.InnerException.StackTrace);
                LogInfo.Log.Error(errMsg, ex);
                return false;
            }
        }
        public void MessageQueue(string Title,string Message)
        {
            StringBuilder sb = new StringBuilder();
            string ProviderName = "ProviderName".ToAppSetting();
            string Address = "MailAdress".ToAppSetting();
            string[] strs = Address.Split(';');
            foreach (var item in strs)
            {
                Message = Message.Replace("'", "''");
                Message = Message.Replace("\"","''");
                sb.AppendLine(string.Format("INSERT INTO BPMSYSMESSAGESQUEUE(PROVIDERNAME,ADDRESS,TITLE,MESSAGE,CREATEAT,FAILCOUNT) VALUES ('{0}','{1}','{2}','{3}',GETDATE(),0);", ProviderName, Address, Title, Message));
            }
            SQLHelper.ExecuteNonQuery(connStr, CommandType.Text, sb.ToString());
        }
        public void MessageQueue(string Address, string Title, string Message)
        {
            StringBuilder sb = new StringBuilder();
            string ProviderName = "ProviderName".ToAppSetting();
            //string Address = "MailAdress".ToAppSetting();
            string[] strs = Address.Split(';');
            foreach (var item in strs)
            {
                Message = Message.Replace("'", "''");
                Message = Message.Replace("\"", "''");
                sb.AppendLine(string.Format("INSERT INTO BPMSYSMESSAGESQUEUE(PROVIDERNAME,ADDRESS,TITLE,MESSAGE,CREATEAT,FAILCOUNT) VALUES ('{0}','{1}','{2}','{3}',GETDATE(),0);", ProviderName, Address, Title, Message));
            }
            SQLHelper.ExecuteNonQuery(connStr, CommandType.Text, sb.ToString());
        }
    }
}
