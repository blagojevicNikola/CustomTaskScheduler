
using MyTaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoSched
{
    class Program
    {
        static void Main(string[] args)
        {
            Scheduler s = new Scheduler(3,2, Scheduler.Mode.PREEMPITVE);
            UserTask a = new UserTask("1",1, 1);
            UserTask b = new UserTask("2", 2, 2);
            UserTask c = new UserTask("3", 1, 1);
            UserTask d = new UserTask("4", 1, 1);
            //UserTask e = new UserTask("5", 2, 1);
            //UserTask f = new UserTask("6", 2, 1);
            MyResource res = new MyResource("FAJL1");
            MyResource res2 = new MyResource("FAJL2");
            a.addResource(res);
            b.addResource(res2);
            c.addResource(res);
            d.addResource(res2);
            a.addResource(res2);
            b.addResource(res);
            c.addResource(res2);
            d.addResource(res);
            s.subscribeUserTask(a);
            Thread.Sleep(3000);
            s.subscribeUserTask(b);
            Thread.Sleep(3000);
            s.subscribeUserTask(c);
            Thread.Sleep(3000);
            s.subscribeUserTask(d);


            //u.cancleUserTask();
            Console.ReadLine();
        }
    }
}
