using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panosen.Transactions.MSTest
{
    [TestClass]
    public class SingleTest
    {
        public class TestCase
        {
            public int Name { get; set; }
            public bool PrepareSuccess { get; set; }
            public bool CommitSuccess { get; set; }
            public bool RollbackSuccess { get; set; }

            public StringBuilder Actual { get; set; } = new StringBuilder();

            public TestCase Clone()
            {
                return new TestCase
                {
                    Name = this.Name,
                    PrepareSuccess = this.PrepareSuccess,
                    CommitSuccess = this.CommitSuccess,
                    RollbackSuccess = this.RollbackSuccess,
                    Actual = new StringBuilder()
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
            foreach (var testCase in testCases)
            {
                var clone = testCase.Clone();

                using (Transaction transaction = new Transaction())
                {
                    transaction.AddOperation(ToOperation(clone));
                }

                Assert.AreEqual(BuildExpected(testCase), clone.Actual.ToString());
            }
        }

        private static string BuildExpected(TestCase testCase)
        {
            var builder = new StringBuilder();

            if (testCase.PrepareSuccess)
            {
                builder.AppendLine($"prepare {testCase.Name} success.");

                if (testCase.CommitSuccess)
                {
                    builder.AppendLine($"commit {testCase.Name} success.");
                }
                else
                {
                    builder.AppendLine($"commit {testCase.Name} failed.");

                    if (testCase.RollbackSuccess)
                    {
                        builder.AppendLine($"rollback {testCase.Name} success.");
                    }
                    else
                    {
                        builder.AppendLine($"rollback {testCase.Name} failed.");
                    }
                }
            }
            else
            {
                builder.AppendLine($"prepare {testCase.Name} failed.");

                if (testCase.RollbackSuccess)
                {
                    builder.AppendLine($"rollback {testCase.Name} success.");
                }
                else
                {
                    builder.AppendLine($"rollback {testCase.Name} failed.");
                }
            }

            return builder.ToString();
        }

        private static Operation ToOperation(TestCase testCase)
        {
            var operation = new Operation();

            operation.OnPrepare(() =>
            {
                if (testCase.PrepareSuccess)
                {
                    testCase.Actual.AppendLine($"prepare {testCase.Name} success.");
                }
                else
                {
                    testCase.Actual.AppendLine($"prepare {testCase.Name} failed.");
                }

                return testCase.PrepareSuccess;
            });

            operation.OnCommit(() =>
            {
                if (testCase.CommitSuccess)
                {
                    testCase.Actual.AppendLine($"commit {testCase.Name} success.");
                }
                else
                {
                    testCase.Actual.AppendLine($"commit {testCase.Name} failed.");
                }

                return testCase.CommitSuccess;
            });

            operation.OnRollback(() =>
            {
                if (testCase.RollbackSuccess)
                {
                    testCase.Actual.AppendLine($"rollback {testCase.Name} success.");
                }
                else
                {
                    testCase.Actual.AppendLine($"rollback {testCase.Name} failed.");
                }
            });

            return operation;
        }
    }
}
