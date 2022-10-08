﻿using MyTaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerTest
{
    public class MockTaskMultipleResources : UserTask
    {
        public MockTaskMultipleResources(string name, int priority, int degreeOfParallelism) : base(name, priority, degreeOfParallelism)
        {
        }

        public MockTaskMultipleResources(string name, int priority, int degreeOfParallelism, long cancellationTimeout) : base(name, priority, degreeOfParallelism, cancellationTimeout)
        {

        }

        public MockTaskMultipleResources(string name, int priority, int degreeOfParallelism, DateTime deadline) : base(name, priority, degreeOfParallelism, deadline)
        {

        }

        public MockTaskMultipleResources(string name, int priority, int degreeOfParallelism, long cancellationTimeout, DateTime deadline) : base(name, priority, degreeOfParallelism, cancellationTimeout, deadline)
        {

        }
        public override void algoritam()
        {
            Console.WriteLine("=====TASK {0} POCINJE=====", base.getName());
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = getDegreeOfParallelism();
            //Parallel.For(0, 15,options, (i, state) =>
            //  {
            //      if (paused)
            //      {
            //          pauseHandle.WaitOne();
            //      }
            //      if (cancleTokenSource.IsCancellationRequested)
            //      {
            //          state.Stop();
            //      }
            //      if (preempted)
            //      {
            //          Console.WriteLine("Preemptovan sam");
            //          preemptHandle.WaitOne();
            //      }
            //      lockResourceByIndex(0);
            //      Console.WriteLine("Task {0} | Prioritet {1} prije zakljucavanja!", name, priority, Thread.CurrentThread.ManagedThreadId);
            //      //lockResourceByIndex(0);
            //      //lockResourceByIndex(1);
            //      //Console.WriteLine("Task {0} je zakljucao resurs!", name);
            //      Thread.Sleep(2000);
            //      Console.WriteLine("Task {0} | Prioritet {1} se izvrsava na Threadu: {2}", name, priority, Thread.CurrentThread.ManagedThreadId);
            //      //unlockResourceByIndex(1);
            //      unlockResourceByIndex(0);
            //      progressOfTask.Report((int)(100 / 15));
            //  });

            for (int i = 0; i < 3; i++)
            {
                if (paused)
                {
                    pauseHandle.WaitOne();
                }
                if (cancleTokenSource.IsCancellationRequested)
                {
                    return;
                }
                if (preempted)
                {
                    Console.WriteLine("Preemptovan sam");
                    preemptHandle.WaitOne();
                }
                lockResourceByIndex(0);
                Console.WriteLine("Task {0} | Prioritet {1} prije zakljucavanja!", name, priority, Thread.CurrentThread.ManagedThreadId);
                lockResourceByIndex(0);
                lockResourceByIndex(1);
                //Console.WriteLine("Task {0} je zakljucao resurs!", name);
                Thread.Sleep(1000);
                Console.WriteLine("Task {0} | Prioritet {1} se izvrsava na Threadu: {2}", name, priority, Thread.CurrentThread.ManagedThreadId);
                unlockResourceByIndex(1);
                unlockResourceByIndex(0);
                progressOfTask.Report((int)(100 / 7));
            }
            Console.WriteLine("-----KRAJ TASKA {0}-----", name); ;
        }
    }
}
