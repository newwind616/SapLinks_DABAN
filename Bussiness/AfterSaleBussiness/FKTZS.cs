using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public class FKTZS
    {
        private string Company { get; set; }
        private string SapCD { get; set; }
        private string ConnStr { get; set; }
        private string ApplyNoPrefix { get; set; }
        private string FileNamePrefix { get; set; }
        private string FileNameSuffix { get; set; }
        private string FilePath { get; set; }
        public FKTZS(string company,string sapCD,string connStr,string applyNoPrefix,string fileNamePrefix,string fileNameSuffix,string filePath)
        {
            this.Company = company;
            this.SapCD = sapCD;
            this.ConnStr = connStr;
            this.ApplyNoPrefix = applyNoPrefix;
            this.FileNamePrefix = fileNamePrefix;
            this.FileNameSuffix = fileNameSuffix;
            this.FilePath = filePath;
        }
        public void GetData()
        {
            #region 获取文件
            ApplyNoBasicEntity applyNoBasicEntity = new ApplyNoBasicEntity();
            applyNoBasicEntity.ApplyNoPrefix = ApplyNoPrefix;//"DSC04";
            applyNoBasicEntity.Company = Company;//"DSC";
            applyNoBasicEntity.ConnStr = ConnStr;
            applyNoBasicEntity.LinksType = new LinkType(LinkType.LinksFalse);
            //applyNoBasicEntity.FktzsProcessType = new FKTZSProcessType(FKTZSProcessType.FKTZS_YC);
            //applyNoBasicEntity.FktzsYcWcType = new FKTZSYCWCType(FKTZSYCWCType.YC);
            IFKTZSMainStartApp fktzsMainStartApp = new FKTZSMainStartApp();
            List<AccVouch> listAccvouch = fktzsMainStartApp.Load(applyNoBasicEntity);
            AccvouchEntityConvertStreamBaseManager accvouchEntityConvertStreamBaseManager = new AccvouchEntityConvertStreamBaseManager();
            StringBuilder sb = new StringBuilder();
            foreach (AccVouch item in listAccvouch)
            {
                sb.AppendLine(accvouchEntityConvertStreamBaseManager.GetVouchString(item));
            }
            #endregion
            #region 文件保存1
            SaveFileEntityCollection saveFileEntityCollection = new SaveFileEntityCollection();
            saveFileEntityCollection.UpdateLinkQueueSql = GetUpdateSapLinkQueue(fktzsMainStartApp.GetApplyNoEntitys());//获取队列applyNo集合
            SaveFileEntity saveFileEntity = new SaveFileEntity();
            saveFileEntity.SetFileNamePrefix(FileNamePrefix).SetFileNameMiddle(string.Format("_{0}_{1}", SapCD, DateTime.Now.ToString("yyyyMMddHHmm")))
                .SetFileNameSuffix(FileNameSuffix).SetFilePath(FilePath).SetContent(sb.ToString());
            saveFileEntityCollection.Add(saveFileEntity);
            ISaveFileProvider iSaveFileProvider = new SaveFileDBProvider(ConnStr);
            iSaveFileProvider.SaveFile(saveFileEntityCollection);
            #endregion
            #region 保存文件2
            
            #endregion
        }
        private string GetUpdateSapLinkQueue(ApplyNoEntityCollection applyNoEntityCollection)
        {
            T_SAP_COMPANYFUNDS_LINKS_QUEUE tquere = new T_SAP_COMPANYFUNDS_LINKS_QUEUE();
            foreach (ApplyNoEntity applyNoEntity in applyNoEntityCollection)
            {
                tquere.Add(applyNoEntity.ApplyNo);
            }
            return tquere.GetSql_ISLINK1();
        }
    }
}
