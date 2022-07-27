using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySched
{
    public abstract class MyTask
    {
        public enum Stanje
        {
            READY,
            RUNNING,
            COMPLETED,
            PREEMPTED
        }

        private Action a;
        protected CancellationTokenSource tokenSource = new CancellationTokenSource();
        protected volatile bool pauziran = false;
        protected EventWaitHandle pauzaHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        protected String ime;
        private int prioritet;
        private int tempPrioritet;
        private Stanje stanje = Stanje.READY;
        private List<MyResource> resursiList = new List<MyResource>();
        private List<MyResource> zauzetiResursiList = new List<MyResource>();
        private readonly object prioritetLck = new object();
        private readonly object resursiLck = new object();
        private MyTs.SchedulerMode schedMod;
        private int degreeOfParallel;

        public MyTask(String ime, int prioritet, int degreeOfParallel)
        {
            //a = algoritam;
            this.ime = ime;
            this.prioritet = prioritet;
            this.degreeOfParallel = degreeOfParallel;
        }

        public MyTask(String ime, int prioritet, List<MyResource> lista, int degreeOfParallel)
        {
            //a = algoritam;
            this.ime = ime;
            this.prioritet = prioritet;
            resursiList = lista;
            this.degreeOfParallel = degreeOfParallel;
            //this.resurs = resurs;
            //this.resurs.setMaxPrioritet(this.prioritet);
        }


        public void algo()
        {
            MyResource fajl1 = getResourceByName(@"C:\Users\win7\source\repos\Practice\treci.txt");
            MyResource fajl2 = getResourceByName(@"C:\Users\win7\source\repos\Practice\prvi.txt");
            for (int i = 0; i < 15; i++)
            {

                zakljucajResurs(fajl1);
                zakljucajResurs(fajl2);
                CancellationToken tempToken = tokenSource.Token;
                if (tempToken.IsCancellationRequested)
                {
                    Console.WriteLine("Task [{0}] je cancel-ovan!", ime);
                    otkljucajResurs(fajl2);
                    otkljucajResurs(fajl1);
                    break;
                }
                if (pauziran)
                {
                    Console.WriteLine("Task [{0}] je pauziran!", ime);
                    pauzaHandle.WaitOne();
                    Console.WriteLine("Task [{0}] je odpauziran!", ime);
                }


                Console.WriteLine("Task [{0}] na tredu: {1}", ime, Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine(getSadrzajResursa(fajl1));
                Console.WriteLine(getSadrzajResursa(fajl2));
                Thread.Sleep(2000);
                otkljucajResurs(fajl2);
                otkljucajResurs(fajl1);


            }

        }

        public abstract void algoritam();

        public void setSchedulerMod(MyTs.SchedulerMode s)
        {
            schedMod = s;
        }

        public MyTs.SchedulerMode getSchedulerMod()
        {
            return schedMod;
        }

        public void reset(String ime, int prioritet, List<MyResource> lista, MyTs.SchedulerMode s, int degreeOfParallel)
        {
            tokenSource = new CancellationTokenSource();
            pauziran = false;
            pauzaHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            this.ime = ime;
            this.prioritet = prioritet;
            tempPrioritet= 0;
            stanje = Stanje.READY;
            resursiList = lista;
            zauzetiResursiList = new List<MyResource>();
            schedMod = s;
            this.degreeOfParallel = degreeOfParallel;
    }

        public String getIme()
        {
            return ime;
        }

        public void setStanje(Stanje s)
        {
            stanje = s;
        }

        public Stanje getStanje()
        {
            return stanje;
        }

        public List<MyResource> getListaResursa()
        {
            return resursiList;
        }

        public void setPrioritet(int prioritet)
        {
            lock (prioritetLck)
            {
                this.prioritet = prioritet;
            }

        }

        public int getPrioritet()
        {
            lock (prioritetLck)
            {
                return prioritet;
            }

        }

        public CancellationTokenSource getTokenSource()
        {
            return tokenSource;
        }

        public void stopMyTask(Stanje s)
        {
            this.setStanje(s);
            tokenSource.Cancel();
        }

        public void pauzirajMyTask()
        {
            pauziran = true;
        }

        public Action getMyTaskAction()
        {
            return a;
        }

        public void odpauzirajMyTask()
        {
            pauziran = false;
            pauzaHandle.Set();
        }

        //public MyResource getResurs()
        //{
        //    return resurs;
        //}

        //public void setResrus(MyResource resurs)
        //{
        //    this.resurs = resurs;
        //}

        //public void zakljucajResurs()
        //{
        //    if(resurs != null)
        //    {
        //        tempPrioritet = getPrioritet();
        //        setPrioritet(resurs.getMaxPrioritet());
        //        resurs.lockResource();
        //    }

        //}

        //public void otkljucajResurs()
        //{
        //    if(resurs != null)
        //    {
        //        resurs.unlcokResource();
        //        setPrioritet(tempPrioritet);
        //    }

        //}

        public String getSadrzajResursa(MyResource res)
        {
            //MyResource res = zauzetiResursiList.Find(r => r.getPutanja() == ime);
            if (res != null)
            {
                return res.getContent();
            }
            else
            {
                return "File not found!";
            }
        }

        public void dodajResurs(MyResource myResource)
        {
            myResource.setMaxPrioritet(this.getPrioritet(), this);

            lock (resursiLck)
            {
                resursiList.Add(myResource);
                resursiList.Sort((MyResource x, MyResource y) =>
                {
                    return x.getId().CompareTo(y.getId());
                });
            }

        }

        public void ukloniResurs(MyResource myResource)
        {
            resursiList.Remove(myResource);
        }

        public void zakljucajResurs(MyResource res)
        {
            //MyResource res = resursiList.Find(r => r.getPutanja() == ime);
            if (res != null)
            {
                if (zauzetiResursiList.Count > 0)
                {
                    List<MyResource> temp = new List<MyResource>();
                    foreach (MyResource resource in zauzetiResursiList)
                    {
                        resource.unlcokResource();
                        temp.Add(resource);
                    }
                    zauzetiResursiList.Clear();
                    temp.Add(res);
                    temp.Sort((t1, t2) => t1.getId().CompareTo(t2.getId()));
                    foreach (MyResource resource in temp)
                    {
                        
                       // Console.WriteLine("-----Task {0} pokusava zakljucati!-----", getIme());
                        resource.lockResource();
                        if(getSchedulerMod() == MyTs.SchedulerMode.PREEMPTIVE)
                        {
                            tempPrioritet = getPrioritet();
                            setPrioritet(resource.getMaxPrioritet());
                        }
                        Console.WriteLine("-----Task {0} zakljucao!-----", getIme());
                        zauzetiResursiList.Add(resource);
                    }
                    return;
                }

                
                //Console.WriteLine("-----Task {0} pokusava zakljucati!-----", getIme());
                res.lockResource();
                if (getSchedulerMod() == MyTs.SchedulerMode.PREEMPTIVE)
                {
                    tempPrioritet = getPrioritet();
                    setPrioritet(res.getMaxPrioritet());
                }
                Console.WriteLine("-----Task {0} zakljucao!-----", getIme());
                zauzetiResursiList.Add(res);
            }
        }

        public void otkljucajResurs(MyResource res)
        {
            //MyResource res = zauzetiResursiList.Find(r => r.getPutanja() == ime);
            if(zauzetiResursiList.Count == 0)
            {
                return;
            }
            if (res != null && zauzetiResursiList.Count == 1)
            {
               // Console.WriteLine("-----Task {0} otkljucao!-----", getIme());
                res.unlcokResource();
                zauzetiResursiList.Remove(res);
                if(getSchedulerMod() == MyTs.SchedulerMode.PREEMPTIVE)
                {
                    setPrioritet(tempPrioritet);
                }
                
            }
            else
            {
                int index = zauzetiResursiList.IndexOf(res);
                for (int i = 0; i <= index; i++)
                {
                    // Console.WriteLine("-----Task {0} otkljucao!-----", getIme());
                    zauzetiResursiList[i].unlcokResource();
                    if (getSchedulerMod() == MyTs.SchedulerMode.PREEMPTIVE)
                    {
                        if (i < zauzetiResursiList.Count - 1)
                        {
                            setPrioritet(zauzetiResursiList[i + 1].getMaxPrioritet());
                        }
                        else
                        {
                            setPrioritet(tempPrioritet);
                        }
                    }
                }
                zauzetiResursiList.RemoveRange(0, index + 1);
            }
        }

        public void otkljucajSve()
        {
            foreach (MyResource resource in zauzetiResursiList)
            {
                resource.unlcokResource();
                if (getSchedulerMod() == MyTs.SchedulerMode.PREEMPTIVE)
                {
                    setPrioritet(tempPrioritet);
                }  
            }
            zauzetiResursiList.Clear();
        }

        public MyResource getResourceByName(String ime)
        {
            return resursiList.Find(r => r.getPutanja() == ime);
        }

        public int getDegreeOfParallel()
        {
            return degreeOfParallel;
        }
    }
}
