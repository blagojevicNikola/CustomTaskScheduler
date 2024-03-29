﻿using MyTaskScheduler;
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
        public string Option { get; set; }
        public string NumberOfCores { get; set; }
        public string LevelOfParalelism { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Subscribers = new ObservableCollection<UserTask>();
            //UserTask a = new NewTask("IME1", 2, 2);
            //UserTask b = new NewTask("IME2", 3, 2, 3000);
            //UserTask c = new NewTask("IME3", 3, 1);
            //UserTask d = new NewTask("IME4", 2, 1);
            ////UserTask c = new NewTask("IME3", 3, 1);
            ////MyResource r = new MyResource("fajl 1");
            //a.addResource(MyResource.getResourceByName("fajl1"));
            //b.addResource(MyResource.getResourceByName("fajl2"));
            //c.addResource(MyResource.getResourceByName("fajl1"));
            //Subscribers.Add(a);
            //Subscribers.Add(b);
            //Subscribers.Add(c);
            //Subscribers.Add(d);
            _scheduler = new Scheduler(2, 2, Scheduler.Mode.PREEMPITVE, TaskScheduler.FromCurrentSynchronizationContext());
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
                if(Option.Contains("Non-Preemptive"))
                {
                    _scheduler.changeMode(Scheduler.Mode.NON_PREEMPTIVE);
                }
                else
                {
                    _scheduler.changeMode(Scheduler.Mode.PREEMPITVE);
                }
                if(string.IsNullOrEmpty(NumberOfCores) || string.IsNullOrEmpty(LevelOfParalelism))
                {
                    return;
                }
                if(!int.TryParse(NumberOfCores, out _) || !int.TryParse(LevelOfParalelism, out _))
                {
                    return;
                }
                _scheduler.setOptions(int.Parse(LevelOfParalelism), int.Parse(NumberOfCores));
                Console.WriteLine("Ispis");
                _scheduler.start();
            }
        }

        private void stopSchedulerButton_Click(object sender, RoutedEventArgs e)
        {
            _scheduler.stop();
        }
    }
}
