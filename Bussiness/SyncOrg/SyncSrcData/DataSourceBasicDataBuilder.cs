using SAPLinks.Bussiness.SyncOrg.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SyncOrg.SyncSrcData
{
    /// <summary>
    /// 解析三方数据源
    /// </summary>
    public abstract class DataSourceBasicDataBuilder:IDataSourceDataBuilder
    {
        public DataSourceBasicDataBuilder(DataSourceFrameEntity dataSourceFrameEntity)
        {
            this.dataSourceFrameEntity = dataSourceFrameEntity;
        }
        /// <summary>
        /// 数据解析
        /// 从文件、数据库、WebService等
        /// </summary>
        public override void DataAnalysis()
        {
            dataTable = this.dataSourceFrameEntity.dataFrame.CreateDataTableFrame(this.dataSourceFrameEntity.type);//根据类型创建数据源框架
            DoDataAnalysis();
        }
        public abstract void DoDataAnalysis();
    }
}
