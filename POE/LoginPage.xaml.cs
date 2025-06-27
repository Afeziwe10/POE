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
using System.IO;
using System.Media;
using System.Windows.Media.Animation;

namespace CyberSecurityChatbot
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private readonly string userDataFile = "users.txt"; //File storing registered user credentials
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim(); //Get username button
            string password = ShowPasswordCheckBox.IsChecked == true ? VisiblePasswordBox.Text : PasswordBox.Password; //Get visible or hidden password based on checkbox

            //Validate that both fields are filled
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                LoginStatus.Text = "Please enter both username and password.";
                return;
            }

            bool loginSuccessful = false; //Tracks login success status

            //Check if user file exist
            if (File.Exists(userDataFile))
            {
                string[] users = File.ReadAllLines(userDataFile); //Read all registered users
                foreach(var user in users)
                {
                    string[] data = user.Split('|'); //Split user data by pipe symbol
                    if(data.Length>= 3) //Ensures data contains username, email, and password
                    {
                        string storedUsername = data[0].Trim();
                        string storedPassword = data[2].Trim();

                        //Check for matching username and password
                        if (username == storedUsername && password == storedPassword)
                        {
                            loginSuccessful = true;
                            break;
                        }
                    }
                }
            }

            if (loginSuccessful)
            {
                LoginStatus.Text = $"Welcome, {username}!"; //show welcome message
                AnimateSuccess(); //Success animation will appear

                //After a short delay, open the dashboard
                Task.Delay(1500).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        string loggInUser = username;
                        List <string> activityLog = new List<string>(); //initialize activity log
                        DashboardPage dashboardPage = new DashboardPage(username,activityLog ); //pass user details
                        dashboardPage.Show(); // show dashboard
                        this.Close();
                    });
                });
            }
            else
            {
                LoginStatus.Text = "Invalid username or password.";
            }
        }

        //Animates the login status text with a fade-in effect
        private void AnimateSuccess()
        {
            DoubleAnimation fadeIn = new DoubleAnimation(0,1, TimeSpan.FromSeconds(1)); //Create fade-in animation
            LoginStatus.BeginAnimation(OpacityProperty, fadeIn); //Apply animation to login status text
        }

        //Handles show password checkbox checked event
        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VisiblePasswordBox.Text = PasswordBox.Password;
            PasswordBox.Visibility = Visibility.Collapsed;
            VisiblePasswordBox.Visibility = Visibility.Visible;
        }

        //Handles show password checkbox unchecked wvent
        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = VisiblePasswordBox.Text;
            VisiblePasswordBox.Visibility= Visibility.Collapsed;
            PasswordBox.Visibility= Visibility.Visible;
        }

        //Keeps passwordBox updated while typing in the visible box
        private void VisiblePasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordBox.Password= VisiblePasswordBox.Text;
        }

        private void RegisterText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterPage registerPage = new RegisterPage();
            registerPage.Show();
            this.Close();
        }
    }
}
