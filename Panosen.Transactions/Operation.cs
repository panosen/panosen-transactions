using System;
using System.Collections.Generic;
using System.Text;

namespace Panosen.Transactions
{
    /// <summary>
    /// 操作
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// 准备
        /// </summary>
        public Func<bool> Prepare { get; set; }

        /// <summary>
        /// 已准备
        /// </summary>
        public bool Prepared { get; set; }

        /// <summary>
        /// 准备成功
        /// </summary>
        public bool PrepareSuccess { get; set; }

        /// <summary>
        /// 提交事务
        /// </summary>
        public Func<bool> Commit { get; set; }

        /// <summary>
        /// 已提交
        /// </summary>
        public bool Commited { get; set; }

        /// <summary>
        /// 提交成功
        /// </summary>
        public bool CommitSuccess { get; set; }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public Action Rollback { get; set; }

        /// <summary>
        /// 已回滚
        /// </summary>
        public bool Rollbacked { get; set; }
    }

    /// <summary>
    /// OperationExtension
    /// </summary>
    public static class OperationExtension
    {
        /// <summary>
        /// OnPrepare
        /// </summary>
        public static Operation OnPrepare(this Operation operation, Func<bool> prepare)
        {
            operation.Prepare = prepare;

            return operation;
        }

        /// <summary>
        /// OnCommit
        /// </summary>
        public static Operation OnCommit(this Operation operation, Func<bool> commit)
        {
            operation.Commit = commit;

            return operation;
        }

        /// <summary>
        /// OnRollback
        /// </summary>
        public static Operation OnRollback(this Operation operation, Action rollback)
        {
            operation.Rollback = rollback;

            return operation;
        }
    }
}
