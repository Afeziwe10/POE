using CyberSecurityChatbot;
using System.Configuration;
using System.Data;
using System.Windows;

namespace POE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // RegisterPage registerPage = new RegisterPage();
            //registerPage.Show();
            LandingPage landingPage = new LandingPage();
            landingPage.Show();
        }
    }

}
