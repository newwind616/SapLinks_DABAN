namespace SAPLinks
{
    public class MainStartApp
    {
        public MainStartApp(string paramet)
        {
            if (ConfigInfo.FunDic.ContainsKey(paramet))
            {
                LogInfo.Log.Info(string.Format("**********执行【{0}】数据传输开始**********", ConfigInfo.FunDic[paramet].Caption));
                string Name = ConfigInfo.FunDic[paramet].FormPath;
                SAPLinks.Bussiness.IBaseAction main = (SAPLinks.Bussiness.IBaseAction)System.Activator.CreateInstance(System.Type.GetType(Name));
                if (!main.PostEvent("Start"))
                {
                    main.MessageQueue(string.Format("{0}失败", ConfigInfo.FunDic[paramet].Caption), main.ErrMsg());
                }
                LogInfo.Log.Info(string.Format("**********执行【{0}】数据传输完毕**********", ConfigInfo.FunDic[paramet].Caption));
            }
            else
            {
                LogInfo.Log.Info(string.Format("启动参数:{0}无效", paramet));
            }
        }
    }
}
