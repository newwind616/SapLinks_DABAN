using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.SyncOrg.SyncOU
{
    public abstract class OUBuilder
    {
        protected BaseAction context;
        protected DataTable OUDataTable { get; set; }
        public OUBuilder(DataTable oudata, BaseAction baseAction)
        {
            this.OUDataTable = oudata;
            this.context = baseAction;
        }
        /// <summary>
        /// 部门删除
        /// BPMSysOUs中Code在OUDataTable中不存在的
        /// </summary>
        public abstract void DeleteOU();
        /// <summary>
        /// 新增部门
        /// OUDataTable中Code在BPMSysOUs中不存在的
        /// </summary>
        public abstract void NewOU();
        /// <summary>
        /// 部门信息不变且不移动
        /// OUDataTable中Code、Name、Level及POUCode在BPMSysOUs中一致无变化的
        /// </summary>
        public abstract void OUNoMoveNo();
        /// <summary>
        /// 部门信息不变但需要移动
        /// OUDataTable中Code、Name、Level在BPMSysOUs中一致，但POUCode在BPMSysOus中不一致
        /// </summary>
        public abstract void OUNoMoveYes();
        /// <summary>
        /// 部门信息变更但不移动
        /// OUDataTable中Code、Name、Level在BPMSysOUs中不一致，但POUCode在BPMSysOus中一致
        /// </summary>
        public abstract void OUYesMoveNo();
        /// <summary>
        /// 部门信息变更且移动
        /// OUDataTable中Code、Name、Level在BPMSysOUs中不一致且POUCode在BPMSysOus中不一致
        /// </summary>
        public abstract void OUYesMoveYes();
        /// <summary>
        /// 剩余数据为异常数据
        /// </summary>
        public virtual void Exception()
        {

        }
        /// <summary>
        /// 将处理完毕的Code从OUDataTable中移除
        /// </summary>
        /// <param name="listCodes"></param>
        public virtual void RemoveCodeFromOUDataTable(List<string> listCodes)
        {
            for (int j = 0; j < listCodes.Count; j++)
            {
                string code = listCodes[j];
                for (int i = OUDataTable.Rows.Count - 1; i >= 0; i--)
                {
                    if (code == OUDataTable.Rows[i]["OUCode"].ToString())
                    {
                        OUDataTable.Rows.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
