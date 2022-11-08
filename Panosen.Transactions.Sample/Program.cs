using Panosen.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Panosen.Transactions.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();

            Store store = new Store(20);
            Log($"Origin Store:{store.Count}");

            List<Student> students = new List<Student>();

            using (Transaction transaction = new Transaction())
            {
                for (int i = 0; i < 5; i++)
                {
                    var student = new Student(store, i);
                    students.Add(student);

                    transaction.AddOperation(() =>
                    {
                        return student.Prepare();
                    }, () =>
                    {
                        return student.Commit();
                    }, () =>
                    {
                        student.Rollback();
                    });
                }
            }

            Log($"Final Store:{store.Count}");
            foreach (var student in students)
            {
                Log($"{student.Id}: want={student.Want},final={student.Final}");
            }

            Log("FinalTotal=" + (store.Count + students.Sum(v => v.Final)));

            Console.WriteLine("...");
            Console.ReadLine();
        }

        private static void Log(string msg)
        {
            Console.WriteLine(msg);
            Console.WriteLine();
        }
    }
}
