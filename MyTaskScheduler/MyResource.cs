using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyTaskScheduler
{
    public class MyResource
    {

        private static List<MyResource> staticResourceList = new List<MyResource>();

        private static int statId = 0;
        private int id;
        private string path;
        private int maximumPriorityOfOwner = 0;
        private bool locked = false;
        private readonly object monitorObject = new object();
        private UserTask ownerTask;
        private MyResource(string path)
        {
            id = statId++;
            this.path = path;
        }


        public static MyResource getResourceByName(string path)
        {
            if (staticResourceList.Any(a => a.getPath().Equals(path)))
            {
                return staticResourceList.Find(a => a.getPath().Equals(path));
            }
            else
            {
                MyResource newResource = new MyResource(path);
                staticResourceList.Add(newResource);
                return newResource;
            }
        }

        public string getPath()
        {
            return path;
        }

        public int getId()
        {
            return id;
        }

        public int getMaximumPriorityOfOwner()
        {
            return maximumPriorityOfOwner;
        }

        public void setMaximumPriorityOfOwner(int priority)
        {
            if(priority> maximumPriorityOfOwner)
            {
                maximumPriorityOfOwner = priority;
            }
        }


        public void lockResource(UserTask task)
        {
           
            Monitor.Enter(monitorObject);
            setOwnerTask(task);
           
        }

        public void unlockResource(UserTask task)
        {
            Monitor.Exit(monitorObject);
            removeOwnerTask();
        }

        private void setOwnerTask(UserTask task)
        {
            ownerTask = task;
        }

        private void removeOwnerTask()
        {
            ownerTask = null;
        }
    }
}
