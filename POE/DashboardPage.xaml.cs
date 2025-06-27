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

namespace CyberSecurityChatbot
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Window
    {
        private string username;

        private List<string> activityLog;// = new List<string>();

        private string currentUser;
        public DashboardPage(string username, List<string> log)
        {
            InitializeComponent();
            this.username = username;
            currentUser = username;
            activityLog = log;
            WelcomeText.Text = $"Welcome, {username}!";
            
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MyProfileWindow profileWindow = new MyProfileWindow(currentUser, activityLog);
            profileWindow.Show();
        }

        private void QuizButton_Click(Object sender, RoutedEventArgs e)
        {
            QuizWindow quizWindow = new QuizWindow(activityLog);
            quizWindow.Show();
        }

        private void ChatButton_Click (object sender, RoutedEventArgs e)
        {
            ChatbotWindow chatbotWindow = new ChatbotWindow(currentUser, activityLog);
            chatbotWindow.Show();
        }

        private void RemindersButton_Click (Object sender, RoutedEventArgs e)
        {
            RemindersWindow remindersWindow = new RemindersWindow();
            remindersWindow.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoginPage loginPage = new LoginPage();
                loginPage.Show();
                this.Close();
            }
        }
    }
}
