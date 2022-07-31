using Microsoft.Win32;
using MyTaskScheduler;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WPFScheduler.Tasks;

namespace WPFScheduler
{
    /// <summary>
    /// Interaction logic for AddingTask.xaml
    /// </summary>

    public partial class AddingTask : Window
    {

        List<TextBox> inputResourceTextBoxes = new List<TextBox>();
        List<TextBox> outputResourceTextBoxes = new List<TextBox>();
        List<StackPanel> inputStackPanelList = new List<StackPanel>();
        List<StackPanel> outputStackPanelList = new List<StackPanel>();
        List<Button> searchButtons = new List<Button>();

        public AddingTask()
        {
            InitializeComponent();
            for(int i = 0; i < 4; i++)
            {
                StackPanel inputPanel = new StackPanel();
                StackPanel outputPanel = new StackPanel();
                Label inDescription = new Label();
                Label outDescription = new Label();
                StackPanel helpPanel = new StackPanel();
                helpPanel.Orientation = Orientation.Horizontal;
                inDescription.Margin = new Thickness(20, 10, 0, 0);
                inDescription.FontSize = 15;
                inDescription.Content = "Input " + (i + 1);
                outDescription.Content = "Output " + (i + 1);
                outDescription.FontSize = 15;
                outDescription.Margin = new Thickness(20, 10, 0, 0);
                TextBox inputResource = new TextBox();
                inputPanel.SetValue(Grid.RowProperty, i);
                inputPanel.Orientation = Orientation.Vertical;
                //inputResource.Height = 30;
                inputResource.Width = 100;
                inputResource.Margin = new Thickness(20, 0, 0, 0);
                inputResource.Padding = new Thickness(0);
                inputPanel.SetValue(Grid.ColumnProperty, 0);
                inputResource.HorizontalAlignment = HorizontalAlignment.Left;
                inputPanel.VerticalAlignment = VerticalAlignment.Bottom;
                inputResource.FontSize = 15;
                inputResourceTextBoxes.Add(inputResource);
                inputPanel.Children.Add(inDescription);
                helpPanel.Children.Add(inputResource);
                inputStackPanelList.Add(inputPanel);
                Button searchBtn = new Button();
                searchBtn.Height = 25;
                searchBtn.Width = 25;
                searchBtn.Click += searchButton_Click;
                searchBtn.Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"C:\Users\win7\Desktop\lupa.png")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                searchButtons.Add(searchBtn);
                helpPanel.Children.Add(searchBtn);
                inputPanel.Children.Add(helpPanel);
                TextBox outputResource = new TextBox();
                outputPanel.SetValue(Grid.RowProperty, i);
                //outputResource.Height = 30;
                outputResource.Width = 100;
                outputResource.Margin = new Thickness(20, 0, 0, 0);
                outputPanel.SetValue(Grid.ColumnProperty, 1);
                outputResource.HorizontalAlignment = HorizontalAlignment.Left;
                outputPanel.VerticalAlignment = VerticalAlignment.Bottom;
                outputResource.FontSize = 15;
                outputResourceTextBoxes.Add(outputResource);
                outputPanel.Children.Add(outDescription);
                outputPanel.Children.Add(outputResource);
                outputStackPanelList.Add(outputPanel);
            }

        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (taskInfoValidation())
            {
                //MyTaskScheduler.UserTask temp = new NewTask(nameTextBox.Text, int.Parse(prioTextBox.Text), int.Parse(coreNumTextBox.Text));
                UserTask temp = null;
                string val = (taskTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                switch (val)
                {
                    case "CB(Single-Input)":
                        temp = new SingleInputCBTask(nameTextBox.Text, int.Parse(prioTextBox.Text), int.Parse(coreNumTextBox.Text));
                        handleInput(temp, 1);
                        break;

                    case "CB(Multi-Input)":
                        temp = new MultiInputCBTask(nameTextBox.Text, int.Parse(prioTextBox.Text), int.Parse(coreNumTextBox.Text));
                        handleInput(temp, 4);
                        break;

                    default:
                        break;
                }

            ((MainWindow)Application.Current.MainWindow).Subscribers.Add(temp);
                this.Close();
            }
        }

        private void taskTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            string option = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            switch (option)
            {
                case "CB(Single-Input)":
                    gridResourceHolder.Children.Clear();
                    for(int i = 0; i < 1; i++)
                    {
                        gridResourceHolder.Children.Add(inputStackPanelList[i]);
                        gridResourceHolder.Children.Add(outputStackPanelList[i]);
                    }
                    break;

                case "CB(Multi-Input)":
                    gridResourceHolder.Children.Clear();
                    for(int i = 0; i < 4; i++ )
                    {
                        gridResourceHolder.Children.Add(inputStackPanelList[i]);
                        gridResourceHolder.Children.Add(outputStackPanelList[i]);
                    }
                    break;
                default:
                    gridResourceHolder.Children.Clear();
                    break;
            }

        }

        private void handleInput(UserTask temp, int count)
        {
            for(int i = 0; i < count; i++)
            {
                if(inputResourceTextBoxes[i].Text.Length<=0 || outputResourceTextBoxes[i].Text.Length<=0)
                {
                    continue;
                }
                temp.addResource(MyTaskScheduler.MyResource.getResourceByName(inputResourceTextBoxes[0].Text));
                temp.addResource(MyTaskScheduler.MyResource.getResourceByName(outputResourceTextBoxes[0].Text));
            }
        }

        private bool taskInfoValidation()
        {
            if(prioTextBox.Text.Length<=0 || nameTextBox.Text.Length<=0 || coreNumTextBox.Text.Length<=0)
            {
                MessageBox.Show("Information about user task is not valid!");
                return false;
            }
            if(!int.TryParse(prioTextBox.Text, out _) || !int.TryParse(coreNumTextBox.Text, out _))
            {
                MessageBox.Show("Priority/NumOfCores not a number!");
                return false;
            }
            return true;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            if(f.ShowDialog() == true)
            {
                Button bt = (Button)sender;
                int indexOfBtn = searchButtons.IndexOf(bt);
                inputResourceTextBoxes[indexOfBtn].Text = f.FileName;
            }
        }

    }
}
