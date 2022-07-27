//using MySched;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DemoSched
//{
//    class ThirdTask : MyTask
//    {
//        public ThirdTask(String ime, int prioritet, List<MyResource> lista, int degreeOfParallel) : base(ime, prioritet, lista, degreeOfParallel)
//        {

//        }

//        public ThirdTask(String ime, int prioritet, int degreeOfParallel) : base(ime, prioritet, degreeOfParallel)
//        {

//        }


//        public override void algoritam()
//        {
//            //for (int i = 0; i < 15; i++)
//            //{

//            //    Console.WriteLine("Task [{0}] prioritet:{1}!", ime,  getPrioritet());
//            //    CancellationToken tempToken = tokenSource.Token;
//            //    if (tempToken.IsCancellationRequested)
//            //    {
//            //        Console.WriteLine("Task [{0}] je cancel-ovan!", ime);
//            //        break;
//            //    }
//            //    if (pauziran)
//            //    {
//            //        Console.WriteLine("Task [{0}] je pauziran!", ime);
//            //        pauzaHandle.WaitOne();
//            //        Console.WriteLine("Task [{0}] je odpauziran!", ime);
//            //    }


//            //    Console.WriteLine("Task [{0}] na tredu: {1}", ime, Thread.CurrentThread.ManagedThreadId);
//            //    Thread.Sleep(2000);


//            //}
//            //Console.WriteLine("=========KRAJ TASKA [{0}]!==========", ime);
//            Object obj = new object();
//            ParallelOptions options = new ParallelOptions();
//            options.MaxDegreeOfParallelism = getDegreeOfParallel();
//            Parallel.For(0, 15, options,  i =>
//            {
//                lock(obj)
//                {
//                    Console.WriteLine("Task[{0}] na Threadu: {1}", ime, Thread.CurrentThread.ManagedThreadId);
//                }
//                Thread.Sleep(1000);
//            });
//            Console.WriteLine("KRAJ taska {0}", ime);
//        }
//    }
//}
