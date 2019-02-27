using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SyncOrg
{
    public class SyncOrgAction : BaseAction, ICompanyAction
    {
        public void Start()
        {
            DTCProvider dtc = new DTCProvider(Go);
            dtc.TimeOut = 600;//DTC设置超时时间为10分钟
            dtc.Start();
        }
        private void Go()
        {
            #region 从数据源装载数据到内存
            Models.DataSourceFrameEntity dfe = new Models.DataSourceFrameEntity();
            dfe.context = this;
            dfe.dataFrame = new SyncSrcData.DSZ.DataSourceDSZFrame();
            dfe.type = DataSourceType.OU;
            SyncSrcData.IDataSourceDataBuilder dsdb = new SyncSrcData.DSZ.DataSourceDSZDataBuilder(dfe);
            dsdb.DataAnalysis();
            #endregion
            #region 更新组织架构
            SAPLinks.Bussiness.SyncOrg.SyncOU.OUBuilder ouBuilder = new SAPLinks.Bussiness.SyncOrg.SyncOU.DSZ.DSZOUBuilder(dsdb.GetResult(), this);
            SAPLinks.Bussiness.SyncOrg.SyncOU.DSZ.DSZOUDirector director = new SyncOU.DSZ.DSZOUDirector(ouBuilder);
            director.Construct();
            #endregion
        }
    }
}
