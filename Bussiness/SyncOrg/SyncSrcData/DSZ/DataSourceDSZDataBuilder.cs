using SAPLinks.Bussiness.SyncOrg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SyncOrg.SyncSrcData.DSZ
{
    public class DataSourceDSZDataBuilder : DataSourceBasicDataBuilder
    {
        public DataSourceDSZDataBuilder(DataSourceFrameEntity dataSourceFrameEntity) : base(dataSourceFrameEntity)
        {
        }

        public override void DoDataAnalysis()
        {
            //解析文件
        }
    }
}
