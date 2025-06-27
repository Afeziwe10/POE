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
using System.Media;
using System.Text.RegularExpressions;
using System.IO;

namespace CyberSecurityChatbot
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Window
    {
        private readonly string userDataFile = "users.txt"; //File to store registrerd users
       // private readonly SoundPlayer player;
        public RegisterPage()
        {
            InitializeComponent();
           // player = new SoundPlayer("audio\\myAudio.wav");
           // player.Play();
        }

        //Clears placeholder text when a textbox gains focus
        private void RemoveText(object sender, RoutedEventArgs e)
        {
           if (sender is TextBox tb)
            {
                if (tb.Text == "Username" ||  tb.Text == "Email")
                {
                    tb.Text = "";// clear placeholder
                }
            }
        }

        //Adds placeholder text if textbox is empty when losing focus
        private void AddText(object sender, RoutedEventArgs e)
        {
           if (sender is TextBox tb)
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    if(tb.Name == "UsernameTextBox")
                    tb.Text = "Username";
                else if (tb.Name == "EmailTextBox")
                        tb.Text = "Email";
                }
                
            }
        }
        //Hides password placeholder when user start typing
        private void HidePasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        //Shows password placeholder if password box is empty
        private void ShowPasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
                PasswordPlaceholder.Visibility = Visibility.Visible;
        }

        //Hides confirm password
        private void HideConfirmPasswordPlaceholder (object sender, RoutedEventArgs e)
        {
            ConfirmPlaceholder.Visibility = Visibility.Collapsed;
        }

        //show confirm password
        private void ShowConfirmPasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(ConfirmPasswordBox.Password))
                ConfirmPlaceholder.Visibility= Visibility.Visible;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ShowPasswordPlaceholder(sender, e);
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ShowConfirmPasswordPlaceholder(sender, e);
        }

        //Handles Register button click logic
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            //Validate all required fields
            if (username == "Username" ||  email == "Email" || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                StatusMessage.Text = "Please fill in all fields";
                return;
            }
            //Validate email format with regular expression
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                StatusMessage.Text = "Invalid email format.";
                return;
            }

            //Confirms password fields match
            if (password != confirmPassword)
            {
                StatusMessage.Text = "Password do not match.";
                return;
            }

            //Saving User data
            try
            {
                //Load Existing users
                if(File.Exists(userDataFile))
                {
                    string[] existingUsers = File.ReadAllLines(userDataFile);

                    foreach (string line in existingUsers)
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 3)
                        {
                            string existingUsername = parts[0].Trim();
                            string existingEmail = parts[1].Trim();

                            //Check for duplicate username
                            if (username.Equals(existingUsername, StringComparison.OrdinalIgnoreCase))
                            {
                                StatusMessage.Text = "Username already exists.";
                                return;
                            }
                            //Check for duplicate email
                            if (email.Equals(existingEmail, StringComparison.OrdinalIgnoreCase))
                            {
                                StatusMessage.Text = "Email already exists.";
                                return;
                            }
                        }
                    }
                }
                //Append new user to the file
                using(StreamWriter sw = new StreamWriter(userDataFile, true))
                {
                    sw.WriteLine($"{username}| {email}| {password}");
                }
                StatusMessage.Text = "Registration successful!";
              //  player.Stop();
                MessageBox.Show("User registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginPage loginPage = new LoginPage();
                loginPage.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                StatusMessage.Text = $"Error: {ex.Message}";
            }
        }

        //Hamdles Back button click to return to the login page
        private void Back_Click(object sender, RoutedEventArgs e)
        {
           // player.Stop();
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }
    }
}
