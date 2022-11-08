using System;

namespace Panosen.Transactions.Sample
{
    public class Student
    {
        public int Id { get; set; }

        public int Want { get; set; }

        public int Final { get; set; }

        public bool PrepareSuccess { get; set; }

        private Store Store;

        private static readonly Random random = new Random();

        public Student(Store store, int id)
        {
            this.Store = store;
            this.Id = id;
        }

        private void Log(int id, string action, int count, bool success)
        {
            Console.WriteLine($"[stud]:stu_{id}.{action}({count})={success}");
            Console.WriteLine();
        }

        public bool Prepare()
        {
            this.Want = random.Next(1, 10);

            var success = this.Store.Sell(this.Id, this.Want);

            this.PrepareSuccess = success;

            Log(this.Id, "prepare", this.Want, success);

            return success;
        }

        public bool Commit()
        {
            this.Final = this.Want;

            var success = true;

            Log(this.Id, "commit", this.Final, success);

            return success;
        }

        public void Rollback()
        {
            if (!this.PrepareSuccess)
            {
                return;
            }

            this.Store.Return(this.Id, this.Want);

            this.Final = 0;

            Log(this.Id, "rollback", this.Want, true);
        }
    }
}
