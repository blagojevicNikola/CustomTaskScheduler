using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practice
{
    class MyTs : TaskScheduler
    {

        private Dictionary<Task, MyTask> dataBase = new Dictionary<Task, MyTask>();
        private readonly object lockObj = new object();
        private readonly int _maxDegreeOfParallelism;
        private readonly LinkedList<Task> taskoviNaCekanju = new LinkedList<Task>();
        private readonly List<Task> aktivniTaskovi = new List<Task>();
        private int _delegatesQueuedOrRunning = 0;
        private readonly List<Task> waitTaskList = new List<Task>();


        public MyTs(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        protected override void QueueTask(Task task)
        {
            lock(taskoviNaCekanju)
            {
                
                if(_delegatesQueuedOrRunning<_maxDegreeOfParallelism)
                {
                    taskoviNaCekanju.AddLast(task);
                    ++_delegatesQueuedOrRunning;
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
                                --_delegatesQueuedOrRunning;
                                break;
                            }

                            // Get the next item from the queue
                            item = taskoviNaCekanju.First.Value;
                            taskoviNaCekanju.RemoveFirst();
                        }
                        lock(lockObj)
                        {
                            aktivniTaskovi.Add(item);
                        }
                        // Execute the task we pulled out of the queue
                        base.TryExecuteTask(item);

                        lock(lockObj)
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
            lock(dataBase)
            {
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
            lock (lockObj)
            {
                MyTask minimumPriority = null;
                MyTask tempData = null;
                int min = 4;
                for(int i = 0; i < aktivniTaskovi.Count; i++)
                {
                    dataBase.TryGetValue(aktivniTaskovi[i], out tempData);
                    if(tempData.getPrioritet() < min)
                    {
                        minimumPriority = tempData;
                        min = tempData.getPrioritet();
                    }
                }

                if(minimumPriority!=null)
                {
                    //Console.WriteLine("Provjera {0} < {1}!", minimumPriority.getPrioritet(), newTask.getPrioritet());
                    if (newTask!=null && minimumPriority.getPrioritet() < newTask.getPrioritet())
                    {
                        
                        lock (taskoviNaCekanju)
                        {
                            taskoviNaCekanju.AddFirst(task);
                        }
                        minimumPriority.stopMyTask();
                        return;
                    }
                    
                }
            }

            lock(taskoviNaCekanju)
            {
                

                foreach(Task t in taskoviNaCekanju)
                {
                    MyTask tempTask = null;
                    dataBase.TryGetValue(t, out tempTask);
                    if(tempTask.getPrioritet() <= newTask.getPrioritet())
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
            if(newTask.getStanje() == MyTask.Stanje.PREEMPTED)
            {
                this.insertAndRun(new MyTask(newTask.getIme(), newTask.getPrioritet(), newTask.getListaResursa()));
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
            if(data1!=null && data2!=null)
            {
                return data2.getPrioritet().CompareTo(data1.getPrioritet());
            }
            else
            {
                if(data1!=null)
                {
                    return -1;
                }
                else if(data2!=null)
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
