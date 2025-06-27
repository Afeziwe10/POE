using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    /// Interaction logic for LandingPage.xaml
    /// </summary>
    public partial class LandingPage : Window
    {
        private readonly SoundPlayer player;
        public LandingPage()
        {
            InitializeComponent();
            player = new SoundPlayer("audio\\myAudio.wav");
            player.Play();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            LoginPage login = new LoginPage();
            login.Show();
            this.Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            RegisterPage register = new RegisterPage();
            register.Show();
            this.Close();
        }
    }
}
