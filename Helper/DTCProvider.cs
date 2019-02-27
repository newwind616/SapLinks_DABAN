using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SAPLinks
{
    public delegate void DTCProviderStart();
    public delegate void ParameterizedDTCProviderStart(object obj);
    public class DTCProvider
    {
        private TransactionScope transactionScope = null;
        public int TimeOut = 180;
        private DTCProviderStart voidStart;
        private ParameterizedDTCProviderStart parameteStart;
        TransactionOptions transactionOption;
        public DTCProvider(DTCProviderStart start)
        {
            voidStart += start;
        }
        public DTCProvider(ParameterizedDTCProviderStart start)
        {
            parameteStart += start;
        }
        public void Start()
        {
            try
            {
                InitTransactionScope();
                voidStart.Invoke();
                transactionScope.Complete();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseTransactionScope();
            }
        }
        public void Start(object obj)
        {
            try
            {
                InitTransactionScope();
                this.parameteStart.Invoke(obj);
                transactionScope.Complete();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseTransactionScope();
            }
        }
        private void InitTransactionScope()
        {
            TransactionOptions transactionOptions = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TimeSpan.FromSeconds(TimeOut)
            };
            transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions);
        }
        private void CloseTransactionScope()
        {
            if (transactionScope != null)
                transactionScope.Dispose();
            if (this.parameteStart != null)
                this.parameteStart = null;
        }
    }
}
