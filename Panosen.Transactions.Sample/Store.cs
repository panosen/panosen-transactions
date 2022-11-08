using System;

namespace Panosen.Transactions.Sample
{
    public class Store
    {
        public int Count { get; set; }

        private readonly object lockObject = new object();

        public Store(int count)
        {
            this.Count = count;
        }

        private void Log(int id, string action, int count, bool success)
        {
            Console.WriteLine($"[store]:stu_{id}.{action}({count})={success}");
            Console.WriteLine();
        }

        public bool Sell(int id, int count)
        {
            lock (lockObject)
            {
                if (count > this.Count)
                {
                    Log(id, "sell", count, false);
                    return false;
                }

                this.Count = this.Count - count;

                Log(id, "sell", count, true);
                return true;
            }
        }

        public void Return(int id, int count)
        {
            Log(id, "return", count, true);
            lock (lockObject)
            {
                this.Count = this.Count + count;
            }
        }
    }
}
