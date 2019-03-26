using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SalesForceToDABAN
{
    public class SalesForce_Action : SalesForceObject
    {
        public SalesForce_Action()
        {
            _filePath = "SalesForce_Path".ToAppSetting();
            _fileName = "SalesForce_Name".ToAppSetting() + DateTime.Now.ToString("yyyyMMddHHmmss");
            _fileExt = "SalesForce_Ext".ToAppSetting();
        }

        public void Start()
        {
            DataConvert SalesForce = S_SalesForce();
            //文件拼接
            string fileData = SalesForce.file_sb.ToString();
            MainFile.WriteFile_(filePath, fileName, fileData);
           // return;
        }

        private DataConvert S_SalesForce()
        {
            LogInfo.Log.Info("《SalesForceToDaBan》数据装载");
            DataConvert entity = new SalesForce(this);
            entity.GetData();
            return entity;
        }
    }
}
