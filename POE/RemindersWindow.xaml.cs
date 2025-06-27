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
using System.Collections.ObjectModel;
using CyberSecurityChatbot;


namespace CyberSecurityChatbot
{
    /// <summary>
    /// Interaction logic for RemindersWindow.xaml
    /// </summary>
    public partial class RemindersWindow : Window
    {
        
        
        //Colection to hold reminders objects
       private List<Reminder> reminders = new List<Reminder>();
        public RemindersWindow()
        {
            InitializeComponent();
            RefreshList(); //Display current reminders
        }

        public void AddReminder(Reminder newReminder)
        {
            reminders.Add(newReminder); //Add reminder to the list
            RefreshList();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            RemindersInputWindow inputWindow = new RemindersInputWindow();
            if (inputWindow.ShowDialog() == true)
            {
                
                reminders.Add(inputWindow.NewReminder);
                RefreshList();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if a reminder is selected from the list
            if (RemindersListView.SelectedItems is Reminder selectedReminder)
            {
                RemindersInputWindow inputWindow = new RemindersInputWindow(selectedReminder);
                if (inputWindow.ShowDialog() == true)
                {
                    //Update reminder details with new input values
                    selectedReminder.Title = inputWindow.TitleInput;
                   selectedReminder.Description = inputWindow.DescriptionInput;
                   selectedReminder.DueDate = inputWindow.DueDateInput;
                        
                       
                 

                    RefreshList();
                }
            }
            else
            {
                MessageBox.Show("Please select a reminder to edit.", "Edit Reminder", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(Object sender, RoutedEventArgs e)
        {
            //Check if a reminder is selected for deletion
            if (RemindersListView.SelectedItem is Reminder selectedReminder)
            {
                reminders.Remove(selectedReminder);
                RefreshList();
            }
            else
            {
                MessageBox.Show("Please select a reminder to delete.", "Delete Reminder", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshList()
        {
            RemindersListView.ItemsSource = null; //Reset the list view 
            RemindersListView.ItemsSource = reminders; //Rebind updated list
        }
    }
}
