using MyTaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practice
{
    class Program
    {
        static void Main(string[] args)
        {
            //MyTask poseban = new MyTask("poseban",4);

            //MyTs scheduler = new MyTs(2);
            ////for (int i = 4; i > 0; i--)
            ////{
            ////    if (i == 3)
            ////    {
            ////        scheduler.insertAndRun(poseban);
            ////    }
            ////    else
            ////    {
            ////        scheduler.insertAndRun(new MyTask(i.ToString(), i));
            ////    }

            ////}
            //MyResource resurs = new MyResource(@"C:\Users\win7\source\repos\Practice\treci.txt");
            //MyResource resurs2 = new MyResource(@"C:\Users\win7\source\repos\Practice\prvi.txt");
            ////MyTask prvi = new MyTask("1", 1, new MyResource(@"C:\Users\win7\source\repos\Practice\prvi.txt"));

            //MyTask treci = new MyTask("3", 2);
            //MyTask cetvrti = new MyTask("4", 1);
            //treci.dodajResurs(resurs);
            //treci.dodajResurs(resurs2);
            //cetvrti.dodajResurs(resurs);
            //cetvrti.dodajResurs(resurs2);

            ////scheduler.insertAndRun(prvi);
            ////Thread.Sleep(2000);
            ////scheduler.insertAndRun(drugi);
            ////Thread.Sleep(2000);
            //scheduler.insertAndRun(treci);
            //Thread.Sleep(2000);
            //scheduler.insertAndRun(cetvrti);
            ////Thread.Sleep(1000);
            ////MyTask drugi = new MyTask("2", 3);
            ////drugi.dodajResurs(resurs);
            ////scheduler.insertAndRun(drugi);
            ////Thread.Sleep(5000);
            ////poseban.pauzirajMyTask();
            ////Thread.Sleep(5000);
            ////poseban.odpauzirajMyTask();
            ////Thread.Sleep(3000);
            ////poseban.pauzirajMyTask();
            //scheduler.waitMyTasks();

            ////Console.WriteLine("Sve je zavrseno!");
            //Console.ReadLine();

            //Klasa k = new Klasa();
            //Stopwatch s = new Stopwatch();
            //s.Start();
            //k.parallelPhotoBlur();
            //s.Stop();
            //Console.WriteLine("Gotovo\nVrijeme izvrsavanja je:{0}s", s.ElapsedMilliseconds);

            Scheduler s = new Scheduler(2, 2, Scheduler.Mode.PREEMPITVE);
            UserTask a = new NewTask("1", 2, 1);
            UserTask b = new NewTask("2", 2, 1);
            UserTask c = new NewTask("3", 3, 1);
            s.subscribeUserTask(a);
            Thread.Sleep(1000);
            s.subscribeUserTask(b);
            Thread.Sleep(1000);
            s.subscribeUserTask(c);

            Console.ReadLine();
        }


        void func(int a)
        {
            while (true)
            {
                Console.WriteLine("Izvrsava se na thread-u {0}", a);
               
                Thread.Sleep(2000);
            }
        }
    }
}
