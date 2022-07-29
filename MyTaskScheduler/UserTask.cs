using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyTaskScheduler
{
    public abstract class UserTask: INotifyPropertyChanged
    {
        public enum TaskState
        {
            RUNNING,
            CANCELED,
            COMPLETED,
            PREEMTED,
            READY
        }

        protected CancellationTokenSource cancleTokenSource = new CancellationTokenSource();
        protected EventWaitHandle pauseHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        public Thread handler;
        protected int priority;
        private int tempPriority = 0;
        private int degreeOfParallelism;
        protected string name;
        protected bool paused = false;
        private int cancellationTimeout;
        private double _progress = 0.0;
        private volatile bool preempted = false;
        private readonly object boostLock = new object();
        private readonly object resourceLock = new object();
        private List<MyResource> resourceList;
        private List<MyResource> lockedResourceList = new List<MyResource>();
        private Dictionary<MyResource, int> boostedPriorities = new Dictionary<MyResource, int>();
        protected IProgress<double> progressOfTask;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public int Priority
        {
            get
            {
                return priority;
            }

            set
            {
                priority = value;
                OnPropertyChanged();
            }

        }

        public int DegreeOfParallelism
        {
            get
            {
                return degreeOfParallelism;
            }
            set
            {
                degreeOfParallelism = value;
            }
        }

        public double Progress { get { return _progress; } set { _progress = value;
                OnPropertyChanged();
            } }

        public TaskState UserTaskState { get; set; }

        public UserTask(string name, int priority, int degreeOfParallelism)
        {
            this.name = name;
            this.priority = priority;
            this.degreeOfParallelism = degreeOfParallelism;
            resourceList = new List<MyResource>();
            progressOfTask = new Progress<double>((i) => Progress = i);
            UserTaskState = TaskState.READY;
        }

        public UserTask(string name, int priority, int degreeOfParallelism, int cancellationTimeout)
        {
            this.name = name;
            this.priority = priority;
            this.degreeOfParallelism = degreeOfParallelism;
            resourceList = new List<MyResource>();
            this.cancellationTimeout = cancellationTimeout;
            UserTaskState = TaskState.READY;

        }

        public abstract void algoritam();
        //{
            //Console.WriteLine("=====TASK {0} POCINJE=====", name);
            //CancellationToken token = cancleTokenSource.Token;
            //ParallelOptions options = new ParallelOptions();
            //options.MaxDegreeOfParallelism = degreeOfParallelism;
            ////Parallel.For(0, 7, options, (a) =>
            ////{
            ////    if (paused)
            ////    {
            ////        pauseHandle.WaitOne();
            ////    }
            ////    lock (lck)
            ////    {
            ////        Console.WriteLine("Task {0} se izvrsava na Threadu: {1}", name, Thread.CurrentThread.ManagedThreadId);
            ////    }
            ////    Thread.Sleep(2000);
            ////   });
            //for (int i = 0; i < 7; i++)
            //{
            //    if (paused)
            //    {
            //        pauseHandle.WaitOne();
            //    }
            //    if (token.IsCancellationRequested)
            //    {
            //        break;
            //    }
            //    Console.WriteLine("Task {0} | Prioritet {1} prije zakljucavanja!", name, priority, Thread.CurrentThread.ManagedThreadId);
            //    lockResourceByIndex(0);
            //    //lockResourceByIndex(1);
            //    //Console.WriteLine("Task {0} je zakljucao resurs!", name);
            //    Thread.Sleep(2000);
            //    Console.WriteLine("Task {0} | Prioritet {1} se izvrsava na Threadu: {2}", name, priority, Thread.CurrentThread.ManagedThreadId);
            //    //unlockResourceByIndex(1);
            //    unlockResourceByIndex(0);
            //    progressOfTask.Report((100/7)*(i+1));
            //}
            //Console.WriteLine("-----KRAJ TASKA {0}-----", name); ;
        //}

        public void resetUserTask()
        {
            cancleTokenSource = new CancellationTokenSource();
            handler = null;
            paused = false;
            preempted = false;
            UserTaskState = TaskState.READY;
            lockedResourceList.Clear();
        }

        public void addResource(MyResource r)
        {
            r.setMaximumPriorityOfOwner(priority);
            resourceList.Add(r);
        }

        public MyResource getResourceByIndex(int i)
        {
            return resourceList[i];
        }

        public void lockResourceByIndex(int i)
        {
           
            if (lockedResourceList.Count == 0)
            {
                resourceList[i].lockResource(this);
                lock(resourceLock)
                {
                    lockedResourceList.Add(resourceList[i]);
                }
            }
            else
            {
                List<MyResource> tempAquireLockResources = new List<MyResource>();
                lock (resourceLock)
                {
                    foreach (MyResource r in lockedResourceList)
                    {
                        tempAquireLockResources.Add(r);
                    }
                }
                tempAquireLockResources.Add(resourceList[i]);
                lock(resourceLock)
                {
                    for (int j = lockedResourceList.Count - 1; j >= 0; j--)
                    {
                        lockedResourceList[j].unlockResource(this);
                    }
                    lockedResourceList.Clear();
                }
                tempAquireLockResources.Sort((a, b) => a.getId().CompareTo(b.getId()));
                foreach (MyResource r in tempAquireLockResources)
                {
                    r.lockResource(this);
                    lock(resourceLock)
                    {
                        lockedResourceList.Add(r);
                    }
                }
            }
        }

        public void unlockResourceByIndex(int i)
        {
            lock (resourceLock)
            {
                if (lockedResourceList.Any((s) => s.getId() == resourceList[i].getId()))
                {
                    int index = lockedResourceList.IndexOf(resourceList[i]);
                    if (index == lockedResourceList.Count - 1)
                    {
                        removeBoostedPriority(lockedResourceList[index]);
                        lockedResourceList[index].unlockResource(this);
                        lockedResourceList.RemoveAt(index);
                    }
                    else
                    {
                        for (int j = lockedResourceList.Count - 1; j >= index; j--)
                        {
                            removeBoostedPriority(lockedResourceList[j]);
                            lockedResourceList[j].unlockResource(this);
                        }
                        lockedResourceList.RemoveRange(index, lockedResourceList.Count - index);
                    }
                }
            }
        }

        public void unlockAllResources()
        {
            lock (resourceLock)
            {
                foreach (MyResource r in lockedResourceList)
                {
                    removeBoostedPriority(r);
                    r.unlockResource(this);
                }
                lockedResourceList.Clear();
            }
        }

        public int getPriority()
        {
            return priority;
        }

        public void setPriority(int p)
        {
            priority = p;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string n)
        {
            name = n;
        }

        public int getDegreeOfParallelism()
        {
            return degreeOfParallelism;
        }

        public void pauseUserTask()
        {
            paused = true;
        }

        public void resumeUserTask()
        {
            paused = false;
            pauseHandle.Set();
        }

        public void cancleUserTask()
        {
            cancleTokenSource.Cancel();
        }

        public void preemptUserTask()
        {
            Console.WriteLine("Ime taska za gasenje {0}", getName());
                preempted = true;
                cancleTokenSource.Cancel();
            
        }

        public bool getPreemtedFlag()
        {
            return preempted;
        }

        //public void setTempPriority(int p)
        //{
        //    lock(prioLck)
        //    {
        //        priority = p;
        //    }
           
        //}

        //public void getTempPriority()
        //{
        //    lock(prioLck)
        //    {
              
        //    }
            
        //}

        public void boostPriority(int prio, MyResource res)
        {
            //This part is accessed by the controller thread of scheduler
            lock (boostLock)
            {
                if (boostedPriorities.Count == 0)
                {
                    tempPriority = priority;
                }
                if (boostedPriorities.ContainsKey(res))
                {
                    boostedPriorities[res] = prio;
                }
                else
                {
                    boostedPriorities.Add(res, prio);
                }
                Priority = prio;
            }
            
        }


        public void removeBoostedPriority(MyResource res)
        {
            //This part is accessed by the UserTask thread when it frees a resource based on which it got it's priority boosted 
            lock (boostLock)
            {
                int prioTemp = 0;
                bool resourceExists = boostedPriorities.TryGetValue(res, out prioTemp);
                if (resourceExists && prioTemp != 0)
                {
                    boostedPriorities.Remove(res);
                    if (boostedPriorities.Count > 0)
                    {
                        int max = boostedPriorities.Max().Value;
                        priority = max;
                        return;
                    }
                    Priority = tempPriority;
                }
            }
        }


        public List<MyResource> getLockedResources()
        {
            return lockedResourceList;
        }

        public List<MyResource> getAllResources()
        {
            return resourceList;
        }

        public Dictionary<MyResource, int> getBoostedPriorities()
        {
            return boostedPriorities;
        }

        public bool hasResourceLocked(MyResource res)
        {
            lock(resourceLock)
            {
                return lockedResourceList.Any(a => a.getPath().Equals(res.getPath()));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int getCancellationTimeout()
        {
            return cancellationTimeout;
        }

        public void startCancellationTimer()
        {
            cancleTokenSource.CancelAfter(cancellationTimeout);
        }

    }

}
