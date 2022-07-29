using MyTaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFScheduler.Tasks
{
    public class NewTask : UserTask
    {
        public NewTask(string name, int priority, int degreeOfParallelism) : base(name, priority, degreeOfParallelism)
        {
        }

        public override void algoritam()
        {
            Console.WriteLine("=====TASK {0} POCINJE=====", base.getName());

            for (int i = 0; i < 7; i++)
            {
                if (paused)
                {
                    pauseHandle.WaitOne();
                }
                if (cancleTokenSource.IsCancellationRequested)
                {
                    break;
                }
                Console.WriteLine("Task {0} | Prioritet {1} prije zakljucavanja!", name, priority, Thread.CurrentThread.ManagedThreadId);
                //lockResourceByIndex(0);
                //lockResourceByIndex(1);
                //Console.WriteLine("Task {0} je zakljucao resurs!", name);
                Thread.Sleep(2000);
                Console.WriteLine("Task {0} | Prioritet {1} se izvrsava na Threadu: {2}", name, priority, Thread.CurrentThread.ManagedThreadId);
                //unlockResourceByIndex(1);
                //unlockResourceByIndex(0);
                progressOfTask.Report((100 / 7) * (i + 1));
            }
            Console.WriteLine("-----KRAJ TASKA {0}-----", name); ;
        }
    }
}
