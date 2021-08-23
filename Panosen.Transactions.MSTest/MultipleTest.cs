using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panosen.Transactions.MSTest
{
    [TestClass]
    public class MultipleTest
    {
        public class TestCase
        {
            public int Name { get; set; }
            public bool PrepareSuccess { get; set; }
            public bool CommitSuccess { get; set; }
            public bool RollbackSuccess { get; set; }

            public TestCase Clone()
            {
                return new TestCase
                {
                    Name = this.Name,
                    PrepareSuccess = this.PrepareSuccess,
                    CommitSuccess = this.CommitSuccess,
                    RollbackSuccess = this.RollbackSuccess
                };
            }
        }

        private List<TestCase> testCases = new List<TestCase>();

        [TestInitialize]
        public void PrepareTestCases()
        {
            testCases.Add(new TestCase { Name = 0, PrepareSuccess = true, CommitSuccess = true, RollbackSuccess = true });
            testCases.Add(new TestCase { Name = 1, PrepareSuccess = true, CommitSuccess = true, RollbackSuccess = false });
            testCases.Add(new TestCase { Name = 2, PrepareSuccess = true, CommitSuccess = false, RollbackSuccess = true });
            testCases.Add(new TestCase { Name = 3, PrepareSuccess = true, CommitSuccess = false, RollbackSuccess = false });
            testCases.Add(new TestCase { Name = 4, PrepareSuccess = false, CommitSuccess = true, RollbackSuccess = true });
            testCases.Add(new TestCase { Name = 5, PrepareSuccess = false, CommitSuccess = true, RollbackSuccess = false });
            testCases.Add(new TestCase { Name = 6, PrepareSuccess = false, CommitSuccess = false, RollbackSuccess = true });
            testCases.Add(new TestCase { Name = 7, PrepareSuccess = false, CommitSuccess = false, RollbackSuccess = false });
        }


        [TestMethod]
        public void Test()
        {
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                var cloneCases = testCases.OrderBy(v => Guid.NewGuid()).Take(random.Next(1, testCases.Count + 1)).Select(v => v.Clone()).ToList();
                Dictionary<int, Operation> operations = new Dictionary<int, Operation>();

                using (Transaction transaction = new Transaction())
                {
                    foreach (var cloneCase in cloneCases)
                    {
                        var operation = ToOperation(cloneCase);
                        transaction.AddOperation(operation);
                        operations.Add(cloneCase.Name, operation);
                    }
                }

                var prepareSuccess = cloneCases.All(v => v.PrepareSuccess);
                var commitSuccess = prepareSuccess && cloneCases.All(v => v.CommitSuccess);

                if (prepareSuccess)
                {
                    foreach (var cloneCase in cloneCases)
                    {
                        var operation = operations[cloneCase.Name];
                        Assert.AreEqual(true, operation.Prepared);
                        Assert.AreEqual(true, operation.PrepareSuccess);
                    }
                }
                if (commitSuccess)
                {
                    foreach (var cloneCase in cloneCases)
                    {
                        var operation = operations[cloneCase.Name];
                        Assert.AreEqual(true, operation.Commited);
                        Assert.AreEqual(true, operation.CommitSuccess);
                    }
                }

                if (!prepareSuccess || !commitSuccess)
                {
                    foreach (var cloneCase in cloneCases)
                    {
                        var operation = operations[cloneCase.Name];
                        if (operation.Prepared)
                        {
                            Assert.AreEqual(true, operation.Rollbacked);
                        }
                    }
                }
            }
        }

        private static Operation ToOperation(TestCase testCase)
        {
            var operation = new Operation();

            operation.OnPrepare(() =>
            {
                return testCase.PrepareSuccess;
            });

            operation.OnCommit(() =>
            {
                return testCase.CommitSuccess;
            });

            operation.OnRollback(() =>
            {
            });

            return operation;
        }
    }
}
