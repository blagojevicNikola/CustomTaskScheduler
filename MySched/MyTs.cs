using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySched
{
    public class MyTs : TaskScheduler
    {
        public enum SchedulerMode
        { 
            PREEMPTIVE,
            NON_PREEMPTIVE
        }

        [ThreadStatic]
        private static int prosliTask = 0;
        [ThreadStatic]
        private static int ugaseno = 0;
        private Dictionary<Task, MyTask> dataBase = new Dictionary<Task, MyTask>();
        private readonly object lockObj = new object();
        private readonly int _maxDegreeOfParallelism;
        private readonly LinkedList<Task> taskoviNaCekanju = new LinkedList<Task>();
        private readonly List<Task> aktivniTaskovi = new List<Task>();
        private int _delegatesQueuedOrRunning = 0;
        private readonly List<Task> waitTaskList = new List<Task>();
        private SchedulerMode mod = SchedulerMode.PREEMPTIVE;
        private bool gasiTred = false;
        private int brTredovaZaGasenje = 0;
        private EventWaitHandle sacekajGasenje = new EventWaitHandle(false, EventResetMode.AutoReset);


        public MyTs(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public MyTs(int maxDegreeOfParallelism, SchedulerMode mod)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            this.mod = mod;
        }

        public int getDegree()
        {
            return _delegatesQueuedOrRunning;
        }
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        protected override void QueueTask(Task task)
        {
            lock (taskoviNaCekanju)
            {
                Console.WriteLine("Trenutno aktivno {0}", _delegatesQueuedOrRunning);
                if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                {
                    taskoviNaCekanju.AddLast(task);
                    ++_delegatesQueuedOrRunning; 
                    Console.WriteLine("Queue je na Threadu: {0}", Thread.CurrentThread.ManagedThreadId);
                    NotifyThreadPoolOfPendingWork();
                    return;
                }
                //else
                //{
                //    MyTask taskData = null;
                //    dataBase.TryGetValue(task, out taskData);
                //    if(taskData!=null)
                //    {
                //        ubaciTask(task);
                //    }
                //}
            }
            ubaciTask(task);
        }

        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                try
                {
                    // Process all available items in the queue.

                    while (true)
                    {
                       
                        Task item;
                        lock (taskoviNaCekanju)
                        {
                            // When there are no more items to be processed,
                            // note that we're done processing, and get out.
                            if (taskoviNaCekanju.Count == 0)
                            {
                            
                                _delegatesQueuedOrRunning -= prosliTask;
                                Console.WriteLine("Gasenje Threada {0} sa PROSLI TASK {1}", Thread.CurrentThread.ManagedThreadId, prosliTask);
                                break;
                            }
                            if (gasiTred)
                            {
                                --brTredovaZaGasenje;
                                if (brTredovaZaGasenje == 0)
                                {
                                    sacekajGasenje.Set();
                                    Thread.Sleep(100);
                                }
                                //_delegatesQueuedOrRunning -= prosliTask;
                                Console.WriteLine("Prekidanje Threada {0} sa PROSLI TASK {1}", Thread.CurrentThread.ManagedThreadId, prosliTask);
                                break;
                            }

                            // Get the next item from the queue
                            item = taskoviNaCekanju.First.Value;
                            taskoviNaCekanju.RemoveFirst();

                            MyTask tempData = null;
                            dataBase.TryGetValue(item, out tempData);
                            if (prosliTask > 0 && prosliTask >= tempData.getDegreeOfParallel())
                            {
                                for (int i = 0; i < prosliTask - tempData.getDegreeOfParallel(); i++)
                                {
                                    Console.WriteLine("BUDIM NOVI TRED!");
                                    NotifyThreadPoolOfPendingWork();
                                }

                            }
                            else
                            {
                                if (_maxDegreeOfParallelism - _delegatesQueuedOrRunning + 1 < tempData.getDegreeOfParallel())
                                {
                                    brTredovaZaGasenje = tempData.getDegreeOfParallel() - 1;
                                    ugaseno = brTredovaZaGasenje;
                                    gasiTred = true;
                                }
                                if(tempData.getDegreeOfParallel() > 1)
                                {
                                    if((_delegatesQueuedOrRunning + tempData.getDegreeOfParallel() - 1) > _maxDegreeOfParallelism)
                                    {
                                        _delegatesQueuedOrRunning = _maxDegreeOfParallelism;
                                    }
                                    else
                                    {
                                        _delegatesQueuedOrRunning = (_delegatesQueuedOrRunning + tempData.getDegreeOfParallel() - 1);
                                    }
                                }

                            }

                            prosliTask = tempData.getDegreeOfParallel();

                        }

                        if (gasiTred)
                        {
                            sacekajGasenje.WaitOne();
                            gasiTred = false;
                        }

                        lock (lockObj)
                        {
                            aktivniTaskovi.Add(item);
                        }
                        // Execute the task we pulled out of the queue
                        Console.WriteLine("Ovo se izvrsava na Threadu: {0}", Thread.CurrentThread.ManagedThreadId);
                        base.TryExecuteTask(item);

                        lock (lockObj)
                        {
                            aktivniTaskovi.Remove(item);
                        }

                        provjeriZavrseniTask(item);

                    }
                }
                // We're done processing items on the current thread
                finally
                {

                }
            }, null);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            throw new NotImplementedException();
        }

        public void insertAndRun(MyTask myTask)
        {
            lock (dataBase)
            {
                myTask.setSchedulerMod(mod);
                Task temp = new Task(() => myTask.algoritam());
                dataBase.Add(temp, myTask);
                temp.Start(this);
            }
        }

        public void waitMyTasks()
        {
            Task.WaitAll(waitTaskList.ToArray());
        }

        private void ubaciTask(Task task)
        {

            MyTask newTask = null;
            dataBase.TryGetValue(task, out newTask);
            if (mod == SchedulerMode.PREEMPTIVE)
            {
                lock (lockObj)
                {
                    MyTask minimumPriority = null;
                    MyTask tempData = null;
                    int min = 4;
                    for (int i = 0; i < aktivniTaskovi.Count; i++)
                    {
                        dataBase.TryGetValue(aktivniTaskovi[i], out tempData);
                        if (tempData.getPrioritet() < min)
                        {
                            minimumPriority = tempData;
                            min = tempData.getPrioritet();
                        }
                    }

                    if (minimumPriority != null)
                    {
                        //Console.WriteLine("Provjera {0} < {1}!", minimumPriority.getPrioritet(), newTask.getPrioritet());
                        if (newTask != null && minimumPriority.getPrioritet() < newTask.getPrioritet())
                        {
                            lock (taskoviNaCekanju)
                            {
                                taskoviNaCekanju.AddFirst(task);
                            }
                            minimumPriority.stopMyTask(MyTask.Stanje.PREEMPTED);
                            return;
                        }

                    }
                }
            }
            lock (taskoviNaCekanju)
            {
                foreach (Task t in taskoviNaCekanju)
                {
                    MyTask tempTask = null;
                    dataBase.TryGetValue(t, out tempTask);
                    if (tempTask.getPrioritet() < newTask.getPrioritet())
                    {
                        taskoviNaCekanju.AddBefore(taskoviNaCekanju.Find(t), task);
                        return;
                    }
                    if(tempTask.getPrioritet() == newTask.getPrioritet())
                    {
                        taskoviNaCekanju.AddAfter(taskoviNaCekanju.Find(t), task);
                        return;
                    }
                }

                taskoviNaCekanju.AddLast(task);
            }

        }

        private void provjeriZavrseniTask(Task task)
        {
            MyTask newTask = null;
            dataBase.TryGetValue(task, out newTask);
            //lock(taskoviNaCekanju)
            //{
            //    Console.WriteLine("MOZDA_____{0}",_delegatesQueuedOrRunning);
            //    if(newTask.getDegreeOfParallel()>1)
            //    {
            //        Console.WriteLine("PARALELNO_____{0}",_delegatesQueuedOrRunning);

            //        _delegatesQueuedOrRunning = _delegatesQueuedOrRunning - (newTask.getDegreeOfParallel() - 1);
            //        for (int i = 0; i < newTask.getDegreeOfParallel() - 1; i++)
            //        {
            //            Console.WriteLine("Probudi!");
            //            NotifyThreadPoolOfPendingWork();
            //        }
            //    }
            //}
            
            if (newTask.getStanje() == MyTask.Stanje.PREEMPTED)
            {
                newTask.reset(newTask.getIme(), newTask.getPrioritet(), newTask.getListaResursa(), SchedulerMode.PREEMPTIVE, newTask.getDegreeOfParallel());
                this.insertAndRun(newTask);
            }
            else
            {
                newTask.setStanje(MyTask.Stanje.COMPLETED);
            }
        }

        private int uporediPrioritete(Task t1, Task t2)
        {
            MyTask data1 = null;
            MyTask data2 = null;
            dataBase.TryGetValue(t1, out data1);
            dataBase.TryGetValue(t2, out data2);
            if (data1 != null && data2 != null)
            {
                return data2.getPrioritet().CompareTo(data1.getPrioritet());
            }
            else
            {
                if (data1 != null)
                {
                    return -1;
                }
                else if (data2 != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
