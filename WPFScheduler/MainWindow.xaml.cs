using MyTaskScheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFScheduler.Tasks;

namespace WPFScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //ObservableCollection<UserTask> _subscribers = new ObservableCollection<UserTask>();
        public ObservableCollection<UserTask> Subscribers { get; set; }
        public Scheduler _scheduler;

        public Scheduler Scheduler { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Subscribers = new ObservableCollection<UserTask>();
            UserTask a = new SingleInputCBTask("IME1", 1, 1);
            UserTask b = new NewTask("IME2", 2, 1);
            //UserTask c = new NewTask("IME3", 3, 1);
            //MyResource r = new MyResource("fajl 1");
            a.addResource(MyResource.getResourceByName(@"C:\Users\win7\Desktop\blurInput1.jpg"));
            //b.addResource(MyResource.getResourceByName("fajl1"));
            //c.addResource(MyResource.getResourceByName("fajl1"));
            Subscribers.Add(a);
            Subscribers.Add(b);
            //Subscribers.Add(c);
            _scheduler = new Scheduler(1, 1, Scheduler.Mode.NON_PREEMPTIVE);
            _scheduler.ObsInQueue = new ObservableCollection<UserTask>();
            _scheduler.ObsActiveTasks = new ObservableCollection<UserTask>();
            Scheduler = _scheduler;
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            _scheduler.subscribeUserTask((UserTask)btn.DataContext);
            Subscribers.Remove((UserTask)btn.DataContext);
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsWindowOpen<AddingTask>())
            {
                AddingTask window = new AddingTask();
                window.Show();
            }
           
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
           foreach (UserTask u in Subscribers)
           {
                _scheduler.subscribeUserTask(u);
           }
            Subscribers.Clear();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            UserTask t = (UserTask)btn.DataContext;
            t.pauseUserTask();
        }

        private void resumeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            UserTask t = (UserTask)btn.DataContext;
            t.resumeUserTask();
        }

        private void cancleButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            UserTask t = (UserTask)btn.DataContext;
            t.cancleUserTask();
        }

        private void startSchedulerButton_Click(object sender, RoutedEventArgs e)
        {
            if(!_scheduler.Active)
            {
                _scheduler.start();
            }
        }

        private void stopSchedulerButton_Click(object sender, RoutedEventArgs e)
        {
            _scheduler.stop();
        }
    }
}
