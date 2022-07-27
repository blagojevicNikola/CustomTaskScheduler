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

namespace WPFScheduler
{
    /// <summary>
    /// Interaction logic for AddingTask.xaml
    /// </summary>

    public partial class AddingTask : Window
    {

        List<TextBox> inputResourceTextBoxes = new List<TextBox>();
        List<TextBox> outputResourceTextBoxes = new List<TextBox>();

        public AddingTask()
        {
            InitializeComponent();
            for(int i = 0; i < 4; i++)
            {
                TextBox inputResource = new TextBox();
                inputResource.SetValue(Grid.RowProperty, i);
                //inputResource.Height = 30;
                inputResource.Width = 100;
                inputResource.Margin = new Thickness(20, 15, 0, 0);
                inputResource.SetValue(Grid.ColumnProperty, 0);
                inputResource.HorizontalAlignment = HorizontalAlignment.Left;
                inputResource.VerticalAlignment = VerticalAlignment.Bottom;
                inputResource.FontSize = 15;
                inputResourceTextBoxes.Add(inputResource);
                TextBox outputResource = new TextBox();
                outputResource.SetValue(Grid.RowProperty, i);
                //outputResource.Height = 30;
                outputResource.Width = 100;
                outputResource.Margin = new Thickness(20, 15, 0, 0);
                outputResource.SetValue(Grid.ColumnProperty, 1);
                outputResource.HorizontalAlignment = HorizontalAlignment.Left;
                outputResource.VerticalAlignment = VerticalAlignment.Bottom;
                outputResource.FontSize = 15;
                outputResourceTextBoxes.Add(outputResource);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            MyTaskScheduler.UserTask temp = new MyTaskScheduler.UserTask(nameTextBox.Text, int.Parse(prioTextBox.Text), int.Parse(coreNumTextBox.Text));
            string val = (taskTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            switch (val)
            {
                case "CB(Single-Input)":
                    temp.addResource(MyTaskScheduler.MyResource.getResourceByName(inputResourceTextBoxes[0].Text));
                    temp.addResource(MyTaskScheduler.MyResource.getResourceByName(outputResourceTextBoxes[0].Text));
                    break;

                case "CB(Multi-Input)":
                    break;

                default:
                    break;
            }

            ((MainWindow)Application.Current.MainWindow).Subscribers.Add(temp);
            this.Close();
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
                        gridResourceHolder.Children.Add(inputResourceTextBoxes[i]);
                        gridResourceHolder.Children.Add(outputResourceTextBoxes[i]);
                    }
                    break;

                case "CB(Multi-Input)":
                    gridResourceHolder.Children.Clear();
                    for(int i = 0; i < 4; i++ )
                    {
                        gridResourceHolder.Children.Add(inputResourceTextBoxes[i]);
                        gridResourceHolder.Children.Add(outputResourceTextBoxes[i]);
                    }
                    break;
                default:
                    gridResourceHolder.Children.Clear();
                    break;
            }

        }

        private void handleSingleInput()
        {

        }

    }
}
