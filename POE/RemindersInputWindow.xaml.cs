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
using static CyberSecurityChatbot.RemindersWindow;
using CyberSecurityChatbot;

namespace CyberSecurityChatbot
{
    /// <summary>
    /// Interaction logic for RemindersInputWindow.xaml
    /// </summary>
    public partial class RemindersInputWindow : Window
    {
        //Properties to store reminder input values
        public string TitleInput { get; set; } //Holds reminder title
        public string DescriptionInput { get; set; } // Holds reminder description
        public string DueDateInput { get; set; } // Holds reminder Due date

        //Property to store the final reminder object created by the user
        public Reminder NewReminder { get; private set; }
        public RemindersInputWindow()
        {
            InitializeComponent();
        }

        //Constructor to load existing reminder for editing
        public RemindersInputWindow(Reminder reminderToEdit) : this()
        {
            //pre-fill the input fields with existing reminder details
            TitleTextBox.Text = reminderToEdit.Title;
            DescriptionTextBox.Text = reminderToEdit.Description;
            DueDateTextBox.Text = reminderToEdit.DueDate;

            //Store the existing values in proteties for potential modification
            TitleInput = reminderToEdit.Title;
            DescriptionInput = reminderToEdit.Description;
            DueDateInput = reminderToEdit.DueDate;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //Check that titles and due date are not empty
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || string.IsNullOrWhiteSpace(DueDateTextBox.Text))
            {
                MessageBox.Show("Please enter both Title and Due Date.", "Input required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Trim whitespaces and store user inputs
            TitleInput = TitleTextBox.Text.Trim();
            DescriptionInput = DescriptionTextBox.Text.Trim();
            DueDateInput = DueDateTextBox.Text.Trim();

            //Create new reminder object with provided inputs
            NewReminder = new Reminder
            {
                Title = TitleInput,
                Description = DescriptionInput,
                DueDate = DueDateInput,
                IsCompleted = false
            };
            

            DialogResult = true; //Signal successful input to calling window
            Close();    
        }

        private void CancelButton_Click(Object sender, RoutedEventArgs e)
        {
            DialogResult = false; //indicate input was cancelled
            Close();
        }
    }
}
