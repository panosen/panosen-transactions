using System;
using System.Collections.Generic;

namespace Panosen.Transactions
{
    /// <summary>
    /// 事务
    /// </summary>
    public class Transaction : IDisposable
    {
        /// <summary>
        /// 操作
        /// </summary>
        public List<Operation> Operations { get; set; }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (this.Operations == null || this.Operations.Count == 0)
            {
                return;
            }

            //Check
            var check = Check();
            if (!check)
            {
                return;
            }

            //Prepare
            bool prepareSuccess = Prepare();
            if (!prepareSuccess)
            {
                Rollback();
                return;
            }

            //Commit
            var commit = Commit();
            if (!commit)
            {
                Rollback();
            }
        }

        private bool Check()
        {
            foreach (var operation in this.Operations)
            {
                if (operation == null)
                {
                    return false;
                }
                if (operation.Prepare == null)
                {
                    return false;
                }
                if (operation.Commit == null)
                {
                    return false;
                }
                if (operation.Rollback == null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool Prepare()
        {
            foreach (var operation in this.Operations)
            {
                operation.PrepareSuccess = operation.Prepare();
                operation.Prepared = true;
                if (!operation.PrepareSuccess)
                {
                    return false;
                }
            }

            return true;
        }

        private bool Commit()
        {
            foreach (var operation in this.Operations)
            {
                operation.CommitSuccess = operation.Commit();
                operation.Commited = true;
                if (!operation.CommitSuccess)
                {
                    return false;
                }
            }

            return true;
        }

        private void Rollback()
        {
            foreach (var operation in this.Operations)
            {
                if (operation.Prepared)
                {
                    operation.Rollback();
                    operation.Rollbacked = true;
                }
            }
        }
    }

    /// <summary>
    /// TransactionExtension
    /// </summary>
    public static class TransactionExtension
    {
        /// <summary>
        /// 准备
        /// </summary>
        public static Operation Prepare(this Transaction transaction, Func<bool> prepare)
        {
            if (transaction.Operations == null)
            {
                transaction.Operations = new List<Operation>();
            }

            Operation operation = new Operation();
            operation.Prepare = prepare;

            transaction.Operations.Add(operation);

            return operation;
        }

        /// <summary>
        /// 增加操作
        /// </summary>
        public static Transaction AddOperation(this Transaction transaction, Operation operation)
        {
            if (transaction.Operations == null)
            {
                transaction.Operations = new List<Operation>();
            }

            transaction.Operations.Add(operation);

            return transaction;
        }

        /// <summary>
        /// 增加操作
        /// </summary>
        public static Operation AddOperation(this Transaction transaction, Func<bool> prepare, Func<bool> commit = null, Action rollback = null)
        {
            if (transaction.Operations == null)
            {
                transaction.Operations = new List<Operation>();
            }

            Operation operation = new Operation();
            operation.Prepare = prepare;
            operation.Commit = commit;
            operation.Rollback = rollback;

            transaction.Operations.Add(operation);

            return operation;
        }
    }
}
