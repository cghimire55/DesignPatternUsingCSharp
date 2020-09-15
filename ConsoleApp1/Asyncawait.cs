using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApp1
{
    public class Asyncawait
    {
        //static  void Main(string[] args)
        //{
        //    Console.WriteLine("task start");
        //    Start();
        //    Console.WriteLine();
        //    Console.ReadLine();
        //}

        static async  void  Start()
        {
            Task<int> t = new Task<int>(LongRunningTask);
            t.Start();
            await t;
            //LongRunningTask();
           Console.WriteLine("task complete");
            //return 100;
        }
        private static int LongRunningTask()
        {
            for (int i = 1; i <= 100; i++)
            {
                Console.WriteLine(i);
            }
            
            Thread.Sleep(5000);
            return 100;
        }
    }

}
