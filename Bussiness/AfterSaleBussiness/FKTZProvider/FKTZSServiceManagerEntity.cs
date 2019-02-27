using SAPLinks.Bussiness.AfterSaleBussiness.YC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPLinks.Bussiness.AfterSaleBussiness
{
    public delegate List<AccVouch> LoadDataHandler(FKTZSServiceEntity fktzsServiceEntity);
    public class FKTZSServiceManagerEntity
    {
        public event LoadDataHandler loadDataHander;
        public IAccountingSubject iAccountingSubject_InputVatAC;
        public IAccountingSubject iAccountingSubject_CostAdjustmentAC;
        public IAccountingSubject iAccountingSubject_AccountPayableAdvanceReceivedAC;
        public IAccountingSubject iAccountingSubject_InputVATDifferencesTurnOutDebtorAC;
        public IAccountingSubject iAccountingSubject_ActualPayableAC;
        public IAccountingSubject iAccountingSubject_InputVATDifferenceAdjustmentAC;
        public IAccountingSubject iAccountingSubject_InputVATDifferencesTurnOutCreditAC;
        public List<AccVouch> Run(FKTZSServiceEntity fktzsServiceEntity)
        {
            List<AccVouch> listAccVouch = new List<AccVouch>();
            if (this.loadDataHander != null)
            {
                Delegate[] deles = this.loadDataHander.GetInvocationList();
                foreach (Delegate dele in deles)
                {
                    List<AccVouch> list = ((LoadDataHandler)dele).Invoke(fktzsServiceEntity);
                    list.MergeListAccVouch(listAccVouch);
                }
                return listAccVouch;
            }
            else
                throw new Exception("LoadDataHandler无任何方法绑定");
        }
        /// <summary>
        /// 凭证1 进项税
        /// </summary>
        /// <param name="Name">要获取的类型的程序集限定名称。如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll中，则提供由命名空间限定的类型名称就足够了。</param>
        public FKTZSServiceManagerEntity SetInputVatAC(string Name = null)
        {
            if (string.IsNullOrEmpty(Name))
                iAccountingSubject_InputVatAC = new InputVatAC();
            else
                iAccountingSubject_InputVatAC = (IAccountingSubject)System.Activator.CreateInstance(System.Type.GetType(Name));
            return this;
        }
        /// <summary>
        /// 凭证2 成本调整
        /// </summary>
        /// <param name="Name">要获取的类型的程序集限定名称。如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll中，则提供由命名空间限定的类型名称就足够了。</param>
        public FKTZSServiceManagerEntity SetCostAdjustmentAC(string Name = null)
        {
            if (string.IsNullOrEmpty(Name))
                iAccountingSubject_CostAdjustmentAC = new CostAdjustmentAC();
            else
                iAccountingSubject_CostAdjustmentAC = (IAccountingSubject)System.Activator.CreateInstance(System.Type.GetType(Name));
            return this;
        }
        /// <summary>
        /// 凭证3 应付暂清帐
        /// </summary>
        /// <param name="Name">要获取的类型的程序集限定名称。如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll中，则提供由命名空间限定的类型名称就足够了。</param>
        public FKTZSServiceManagerEntity SetAccountPayableAdvanceReceivedAC(string Name = null)
        {
            if (string.IsNullOrEmpty(Name))
                iAccountingSubject_AccountPayableAdvanceReceivedAC = new AccountPayableAdvanceReceivedAC();
            else
                iAccountingSubject_AccountPayableAdvanceReceivedAC = (IAccountingSubject)System.Activator.CreateInstance(System.Type.GetType(Name));
            return this;
        }
        /// <summary>
        /// 凭证4 进项差异转出借方
        /// </summary>
        /// <param name="Name">要获取的类型的程序集限定名称。如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll中，则提供由命名空间限定的类型名称就足够了。</param>
        public FKTZSServiceManagerEntity SetInputVATDifferencesTurnOutDebtorAC(string Name = null)
        {
            if (string.IsNullOrEmpty(Name))
                iAccountingSubject_InputVATDifferencesTurnOutDebtorAC = new InputVATDifferencesTurnOutDebtorAC();
            else
                iAccountingSubject_InputVATDifferencesTurnOutDebtorAC = (IAccountingSubject)System.Activator.CreateInstance(System.Type.GetType(Name));
            return this;
        }
        /// <summary>
        /// 凭证5 实际应付
        /// </summary>
        /// <param name="Name">要获取的类型的程序集限定名称。如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll中，则提供由命名空间限定的类型名称就足够了。</param>
        public FKTZSServiceManagerEntity SetActualPayableAC(string Name = null)
        {
            if (string.IsNullOrEmpty(Name))
                iAccountingSubject_ActualPayableAC = new ActualPayableAC();
            else
                iAccountingSubject_ActualPayableAC = (IAccountingSubject)System.Activator.CreateInstance(System.Type.GetType(Name));
            return this;
        }
        /// <summary>
        /// 凭证6 进项差异调整
        /// </summary>
        /// <param name="Name">要获取的类型的程序集限定名称。如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll中，则提供由命名空间限定的类型名称就足够了。</param>
        public FKTZSServiceManagerEntity SetInputVATDifferenceAdjustmentAC(string Name = null)
        {
            if (string.IsNullOrEmpty(Name))
                iAccountingSubject_InputVATDifferenceAdjustmentAC = new InputVATDifferenceAdjustmentAC();
            else
                iAccountingSubject_InputVATDifferenceAdjustmentAC = (IAccountingSubject)System.Activator.CreateInstance(System.Type.GetType(Name));
            return this;
        }
        /// <summary>
        /// 凭证7 进项差异转出贷方
        /// </summary>
        /// <param name="Name">要获取的类型的程序集限定名称。如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll中，则提供由命名空间限定的类型名称就足够了。</param>
        public FKTZSServiceManagerEntity SetInputVATDifferencesTurnOutCreditAC(string Name = null)
        {
            if (string.IsNullOrEmpty(Name))
                iAccountingSubject_InputVATDifferencesTurnOutCreditAC = new InputVATDifferencesTurnOutCreditAC();
            else
                iAccountingSubject_InputVATDifferencesTurnOutCreditAC = (IAccountingSubject)System.Activator.CreateInstance(System.Type.GetType(Name));
            return this;
        }
    }
}
