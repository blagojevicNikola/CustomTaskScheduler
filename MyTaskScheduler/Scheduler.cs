using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MyTaskScheduler
{
    public class Scheduler
    {

        public enum Mode
        {
            NON_PREEMPTIVE,
            PREEMPITVE
        }

       
        private TaskScheduler context;
        private List<UserTask> subscribedTasks = new List<UserTask>();
        private LinkedList<UserTask> tasksInQueue = new LinkedList<UserTask>();
        //private List<Thread> activeThreads = new List<Thread>();
        private List<UserTask> activeTasks = new List<UserTask>();
        private Mode mode = Mode.PREEMPITVE;
        private int maxDegreeOfParallelism;
        private int activeThreadsCount = 0;
        private int activeTasksCount = 0;
        private int _maxConcurrentTasks;
        private Thread controller;
        private volatile bool active = false;
        private readonly object activeThreadsCountLock = new object();

        public ObservableCollection<UserTask> ObsInQueue { get; set; }
        public ObservableCollection<UserTask> ObsActiveTasks { get; set; }

        public bool Active { get { return active; } set { active = value; } }

        public Scheduler(int maxDegreeOfParallelism, int maxConcurrentTasks, Mode mode)
        {
            if(maxDegreeOfParallelism <= 0)
            {
                throw new ArgumentOutOfRangeException("Out of range!");
            }
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
            this.mode = mode;
            _maxConcurrentTasks = maxConcurrentTasks;
            context = null;
            InitializeScheduler();
        }

        public Scheduler(int maxDegreeOfParallelism, int maxConcurrentTasks, Mode mode, TaskScheduler context)
        {
            if (maxDegreeOfParallelism <= 0)
            {
                throw new ArgumentOutOfRangeException("Out of range!");
            }
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
            this.mode = mode;
            _maxConcurrentTasks = maxConcurrentTasks;
            this.context = context;
            InitializeScheduler();
        }


        private void InitializeScheduler()
        {
            controller = new Thread(() =>
            {
                while (active)
                {
                    //First, Scheduler removes all tasks that are completed or preempted in activeTasks list;
                    if (activeTasksCount > 0)
                    {
                        int removeNum = 0;
                        //foreach (UserTask u in activeTasks)
                        //{
                        //    if (u.getPreemtedFlag())
                        //    {
                        //        //removeNum += u.getDegreeOfParallelism();
                        //        UserTask newTask = new UserTask(u.getName(), u.getPriority(), u.getDegreeOfParallelism());
                        //        foreach (MyResource r in u.getAllResources())
                        //        {
                        //            newTask.addResource(r);
                        //        }
                        //        queueTask(newTask);
                        //        Task dispatch = new Task(()=> ObsInQueue.Add(newTask));
                        //        dispatch.Start(context);
                        //        //activeTasksCount--;
                        //    }
                        //    else if (u.UserTaskState == UserTask.TaskState.COMPLETED)
                        //    {
                        //        removeNum += u.getDegreeOfParallelism();
                        //        activeTasksCount--;
                        //    }
                        //}
                        foreach(UserTask u in activeTasks)
                        {
                            if(u.getCancellationTimeout() < u.getStopwatchValue() || u.getDeadline() < DateTime.Now)
                            {
                                u.cancleUserTask();
                            }
                        }

                        var tempList = activeTasks.Where((s) => s.UserTaskState == UserTask.TaskState.COMPLETED || s.UserTaskState == UserTask.TaskState.PREEMTED);
                        // activeTasks.RemoveAll((s) => s.UserTaskState == UserTask.TaskState.READY || s.UserTaskState == UserTask.TaskState.COMPLETED || s.UserTaskState == UserTask.TaskState.PREEMTED || s.getPreemtedFlag() == true);
                        foreach(UserTask u in tempList.ToList())
                        {
                            activeTasks.Remove(u);
                            if(context!=null)
                            {
                                Task t = new Task(() =>
                                {
                                    ObsActiveTasks.Remove(u);
                                });
                                t.Start(context);
                            }
                            if(u.UserTaskState==UserTask.TaskState.COMPLETED)
                            {
                                removeNum += u.getDegreeOfParallelism();
                                activeTasksCount--;
                            }
                            else if (u.getPreemtedFlag())
                            {
                                u.resetUserTask();
                                queueTask(u);
                                if(context!=null)
                                {
                                    Task dispatch = new Task(() => ObsInQueue.Add(u));
                                    dispatch.Start(context);
                                }
                            }
                        }
                        //Task t = new Task(() => { 
                        //    foreach (UserTask u in tempList)
                        //    {
                        //        ObsActiveTasks.Remove(u); 
                        //    }
                        //});
                        //t.Start(context);
                        activeThreadsCount -= removeNum;
                    }

                    bool newTaskStarted = false;

                    //Then, Scheduler checks if there is any new subscribed tasks in subscribedTasks list;
                    lock (subscribedTasks)
                    {
                        if (subscribedTasks.Count > 0)
                        {
                            UserTask executeCandidate = subscribedTasks.First();
                            subscribedTasks.Remove(executeCandidate);
                            //If there is available space for new task and if it dosn't take more cores than available, run the task, otherwise queue the task in waiting queue
                            if (canBeExecuted(executeCandidate))
                            {
                                runTask(executeCandidate);
                                newTaskStarted = true;
                            }
                            else
                            {
                                queueTask(executeCandidate);
                                if(context!=null)
                                {
                                    Task t = new Task(() => ObsInQueue.Add(executeCandidate));
                                    t.Start(context);
                                }
                                
                                //ObsInQueue.Add(executeCandidate);
                            }
                        }
                    }

                    //In case new task was not started, scheduler checks if any task from waiting queue can be runned
                    if (!newTaskStarted)
                    {
                        if (tasksInQueue.Count() > 0)
                        {
                            //Since waiting queue is sorted in descending order by task priority, scheduler checks if the task with heighest priority is runnable
                            UserTask candidate = tasksInQueue.First();
                            if (canBeExecuted(candidate))
                            {
                                if(context!=null)
                                {
                                    Task t = new Task(() => ObsInQueue.Remove(candidate));
                                    t.Start(context);
                                }
                                //Console.WriteLine("OBRISANO_______");
                                tasksInQueue.RemoveFirst();
                                //In case that candidate task was preempted, scheduler just resumes the thread that task is runned on
                                if(candidate.UserTaskState == UserTask.TaskState.WAITING)
                                {
                                    activeTasks.Add(candidate);
                                    if(context!=null)
                                    {
                                        Task dispatch = new Task(() => ObsActiveTasks.Add(candidate));
                                        dispatch.Start(context);
                                    }
                                    activeTasksCount++;
                                    activeThreadsCount += candidate.getDegreeOfParallelism();
                                    candidate.continueTask();
                                }
                                //Else scheduler will start new thread for the candidate task
                                else
                                {
                                    runTask(candidate);
                                }
                            }
                        }
                    }
                    Thread.Sleep(300);
                }
            });
            controller.IsBackground = true;
        }

        #region Start Task Scheduler
        public void start()
        {
            if(controller.ThreadState == ThreadState.Stopped)
            {
                InitializeScheduler();
            }
            active = true;
            controller.Start();
        }
        #endregion

        #region Stop Task Scheduler
        public void stop()
        {
            active = false;
        }
        #endregion

        #region Queue task in "tasksInQueue" linked list
        private void queueTask(UserTask t)
        {
            bool added = false;
            if(tasksInQueue.Count>0)
            {
                LinkedListNode<UserTask> tempNode = null;
                foreach(UserTask task in tasksInQueue)
                {
                    if(t.getPriority() > task.getPriority())
                    {
                        tempNode = tasksInQueue.Find(task);
                        break;
                    }
                }
                if(tempNode!=null)
                {
                    tasksInQueue.AddBefore(tempNode, t);
                    added = true;
                }
            }

            if(!added)
            {
                tasksInQueue.AddLast(t);
            }

        }
        #endregion

        #region Subscribe new task to Scheduler
        public void subscribeUserTask(UserTask task)
        {
            lock (subscribedTasks)
            {
                subscribedTasks.Add(task);
            }
        }
        #endregion

        #region Check if task can be run on Scheduler (for both Scheduler modes)
        private bool canBeExecuted(UserTask t)
        {
            if (t.UserTaskState != UserTask.TaskState.READY && t.UserTaskState != UserTask.TaskState.WAITING)
            {
                Console.WriteLine("Ime taska {0}", t.getName());
                return false;
            }
            if (activeTasksCount < _maxConcurrentTasks && (t.getDegreeOfParallelism() + activeThreadsCount <= maxDegreeOfParallelism))
            {
                return true;
            }
            else if (mode == Mode.PREEMPITVE)
            {
                int min = -1;
                if (activeTasks.Count > 0)
                { 
                    min = activeTasks.Min(a => a.getPriority());
                }
                if (min == -1 || min >= t.getPriority())
                {
                    return false;
                }

                UserTask preemptCandidate = activeTasks.Find(a => a.getPriority() == min);

                if (t.getDegreeOfParallelism() + (maxDegreeOfParallelism - preemptCandidate.getDegreeOfParallelism()) > maxDegreeOfParallelism)
                {
                    return false;
                }

                //PIP Scheduler checks if preemption candidate has any locked resources that high priority task might use and if it dose, Scheduler boosts
                //the priority of active task
                foreach (MyResource r in t.getAllResources())
                {
                    if (preemptCandidate.hasResourceLocked(r))
                    {
                        preemptCandidate.boostPriority(t.getPriority(), r);
                        return false;
                    }
                }
                //Console.WriteLine("Preempt task {0}", preemptCandidate.getName());

                
                activeTasksCount--;
                activeThreadsCount -= preemptCandidate.getDegreeOfParallelism();
                preemptCandidate.preemptUserTask();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Run task on a ThreadPool
        private void runTask(UserTask task)
        {
            if(context!=null)
            {
                Task t = new Task(() => ObsActiveTasks.Add(task));
                t.Start(context);
            }
            activeTasks.Add(task);
            activeTasksCount++;
            activeThreadsCount += task.getDegreeOfParallelism();
            
            //Console.WriteLine("Pri pokretanju taska {0}, active tasks: {1}, active threads: {2}", task.getName(), activeTasksCount, activeThreadsCount);
            ThreadPool.UnsafeQueueUserWorkItem((_) => {
                try
                {
                    lock (task)
                    {
                       task.UserTaskState = UserTask.TaskState.RUNNING;
                        task.startStopwatch();
                    }
                    task.algoritam();
                    lock (task)
                    {
                        //if (task.getPreemtedFlag())
                        //{
                        //    task.UserTaskState = UserTask.TaskState.PREEMTED;
                        //}
                        //else
                        //{
                            task.UserTaskState = UserTask.TaskState.COMPLETED;
                        //}
                    }
                }
                finally
                {
                    
                }
            }, true);

        }
        #endregion

        public void changeMode(Mode m)
        {
            this.mode = m;
        }

        public void setOptions(int levelOfParallelism, int maxConcurrentTasks)
        {
            this.maxDegreeOfParallelism = levelOfParallelism;
            this._maxConcurrentTasks = maxConcurrentTasks;
        }

    }
}
