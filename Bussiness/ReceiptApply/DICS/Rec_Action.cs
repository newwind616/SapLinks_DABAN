using SAPLinks.Helper.SaveFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.ReceiptApply
{
    public class Rec_Action : BaseAction, ICompanyAction
    {
        public void Start()
        {
            //T_SAP_COMPANYFUNDS_LINKS_QUEUE queue = new T_SAP_COMPANYFUNDS_LINKS_QUEUE();
            Dictionary<string, List<RecList>> dic = new Dictionary<string, List<RecList>>();
            Dictionary<string, T_SAP_COMPANYFUNDS_LINKS_QUEUE> dicQ = new Dictionary<string, T_SAP_COMPANYFUNDS_LINKS_QUEUE>();
   
            //MLog.Log.Info("aaa").Info("11111");
            try
            {
                LogInfo.Log.Info("职工机购买-收据开具SAP连携");
                RecEntity recEntity = new RecEntity();
                recEntity.GetSqlByList(ref dic,ref dicQ);
            }
            catch (Exception ex) {
                LogInfo.Log.Info("职工机购买-收据开具SAP连携——异常:" + ex.Message);
            }
            try
            {
                LogInfo.Log.Info("限时特卖-收据开具SAP连携");
                SpecialSaleEntity sse = new SpecialSaleEntity();
                sse.SyncSap(ref dic, ref dicQ);
            }
            catch (Exception ex)
            {
                LogInfo.Log.Info("限时特卖-收据开具SAP连携——异常:" + ex.Message);
            }

            try
            {
                LogInfo.Log.Info("解限SAP连携");
                JXBatchListEntity jx = new JXBatchListEntity();
                jx.SyncSap(ref dic, ref dicQ);
            }
            catch (Exception ex)
            {
                LogInfo.Log.Info("解限SAP连携——异常:" + ex.Message);
            }

            foreach (var item in dic)
            {
                string filePath = "DICS_Path_Rec".ToAppSetting();
                string fileName = "DICS_Name_Rec".ToAppSetting() + "_" + item.Key + "_" + DateTime.Now.ToString("yyyyMMdd");
                string fileExt = "DICS_Ext_Rec".ToAppSetting();
                List<RecList> recList = dic[item.Key];
                T_SAP_COMPANYFUNDS_LINKS_QUEUE queue = dicQ[item.Key];
                AccvouchEntityConvertStreamBaseManager accvouchEntityConvertStreamBaseManager = new AccvouchEntityConvertStreamBaseManager();
                SapLinksQueueEntity sapLinksQueue = new SapLinksQueueEntity("BPMDB".ToConnectionString(), queue.GetSql_ISLINK1());
                StringBuilder sb = new StringBuilder();
                foreach (RecList rl in recList)
                {
                    sb.AppendLine(rl.ToVouchString());
                }

                SAPLinks.Helper.SaveFile.ISaveFileProvider saveFileProvider = SapSaveFileProviderFactory.CreateSaveProvider(sapLinksQueue);
                saveFileProvider.AddFileInfo(new SapSaveFileInfo(filePath, fileName + fileExt, sb));
                saveFileProvider.SaveFile();
            }
        }
    }
}
