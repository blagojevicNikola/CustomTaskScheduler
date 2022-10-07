using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MyTaskScheduler;
using System.Threading;

namespace SchedulerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TurnOnScheduler()
        {
            Scheduler scheduler = new Scheduler(1, 1, Scheduler.Mode.PREEMPITVE);
            scheduler.start();
            Thread.Sleep(500);
            Assert.AreEqual(scheduler.Active, true);
            Thread.Sleep(500);
            scheduler.stop();
        }

        [TestMethod]
        public void TurnOfScheduler()
        {
            Scheduler scheduler = new Scheduler(1, 1, Scheduler.Mode.PREEMPITVE);
            scheduler.start();
            Thread.Sleep(500);
            scheduler.stop();
            Assert.AreEqual(scheduler.Active, false);
        }

        [TestMethod]
        public void CheckTaskStateBeforeStart()
        {
            MockTask task = new MockTask("Task 1", 1, 1);
            Assert.AreEqual(task.UserTaskState, UserTask.TaskState.READY);
        }

        [TestMethod]
        public void CheckTaskStateWhileRunning()
        {
            MockTask task = new MockTask("Task 1", 1, 1);
            Scheduler scheduler = new Scheduler(2, 2, Scheduler.Mode.NON_PREEMPTIVE);
            scheduler.start();
            Thread.Sleep(500);
            scheduler.subscribeUserTask(task);
            Thread.Sleep(2000);
            Assert.AreEqual(task.UserTaskState, UserTask.TaskState.RUNNING);
            scheduler.stop();
        }

        [TestMethod]
        public void CheckTaskStateAfterItsDone()
        {
            MockTask task = new MockTask("Task 1", 1, 1);
            Scheduler scheduler = new Scheduler(2, 2, Scheduler.Mode.NON_PREEMPTIVE);
            scheduler.start();
            scheduler.subscribeUserTask(task);
            Thread.Sleep(18000);
            Assert.AreEqual(task.UserTaskState, UserTask.TaskState.COMPLETED);
            scheduler.stop();
        }

        [TestMethod]
        public void TestNonPreemptiveSchedulerMode()
        {
            MockTask task1 = new MockTask("Task 1", 1, 1);
            MockTask task2 = new MockTask("Task 2", 2, 1);
            Scheduler scheduler = new Scheduler(1, 1, Scheduler.Mode.NON_PREEMPTIVE);
            scheduler.start();
            scheduler.subscribeUserTask(task1);
            scheduler.subscribeUserTask(task2);
            Thread.Sleep(500);
            Assert.AreEqual(task1.UserTaskState, UserTask.TaskState.RUNNING);
            Assert.AreEqual(task2.UserTaskState, UserTask.TaskState.READY);
            scheduler.stop();
        }

        [TestMethod]
        public void TestPreemptiveSchedulerMode()
        {
            MockTask task1 = new MockTask("Task 1", 1, 1);
            MockTask task2 = new MockTask("Task 2", 2, 1);
            Scheduler scheduler = new Scheduler(1, 1, Scheduler.Mode.PREEMPITVE);
            scheduler.start();
            scheduler.subscribeUserTask(task1);
            scheduler.subscribeUserTask(task2);
            Thread.Sleep(2000);
            Assert.AreEqual(task2.UserTaskState, UserTask.TaskState.RUNNING);
            Assert.AreEqual(task1.UserTaskState, UserTask.TaskState.WAITING);
            scheduler.stop();
        }
        [TestMethod]
        public void TestTryingToRunTaskWithNotEnoughCores()
        {
            MockTask task1 = new MockTask("Task 1", 1, 2);
            Scheduler scheduler = new Scheduler(1, 1, Scheduler.Mode.PREEMPITVE);
            scheduler.start();
            scheduler.subscribeUserTask(task1);
            Thread.Sleep(1000);
            Assert.AreEqual(task1.UserTaskState, UserTask.TaskState.READY);
            scheduler.stop();
        }

        [TestMethod]
        public void TestCancellationOfTask()
        {
            MockTask task = new MockTask("Task 1", 1, 1, 5000);
            Scheduler scheduler = new Scheduler(1, 1, Scheduler.Mode.NON_PREEMPTIVE);
            scheduler.start();
            scheduler.subscribeUserTask(task);
            Thread.Sleep(500);
            Assert.AreEqual(task.UserTaskState, UserTask.TaskState.RUNNING);
            Thread.Sleep(6000);
            Assert.AreEqual(task.UserTaskState, UserTask.TaskState.COMPLETED);
            scheduler.stop();
        }

        [TestMethod]
        public void TestDeadlineOfTask()
        {
            MockTask task = new MockTask("Task 1", 1, 1, DateTime.Now.AddSeconds(5));
            Scheduler scheduler = new Scheduler(1, 1, Scheduler.Mode.NON_PREEMPTIVE);
            scheduler.start();
            scheduler.subscribeUserTask(task);
            Thread.Sleep(500);
            Assert.AreEqual(task.UserTaskState, UserTask.TaskState.RUNNING);
            Thread.Sleep(6000);
            Assert.AreEqual(task.UserTaskState, UserTask.TaskState.COMPLETED);
            scheduler.stop();
        }

        public void Test()
        {

        }
    }
}
