using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SAPLinks.Bussiness.SyncOrg.Models;

namespace SAPLinks.Bussiness.SyncOrg.SyncSrcData
{
    public abstract class IDataSourceDataBuilder
    {
        protected DataTable dataTable;
        protected DataSourceFrameEntity dataSourceFrameEntity;
        public abstract void DataAnalysis();
        /// <summary>
        /// 返回待处理数据结果集
        /// </summary>
        /// <returns></returns>
        public DataTable GetResult()
        {
            return dataTable;
        }
    }
}
