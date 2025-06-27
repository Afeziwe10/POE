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
    /// Interaction logic for MyProfileWindow.xaml
    /// </summary>
    public partial class MyProfileWindow : Window
    {
        public MyProfileWindow(string username, List<string> activityLog)
        {
            InitializeComponent();

            UsernameText.Text = string.IsNullOrEmpty(username) ?"Unknown User": username;
            //Check if there are no activities to display
            if (activityLog != null && activityLog.Count > 0 )
            {
                int count = 1;
                foreach(string activity in activityLog)
                {
                   ActivityList.Items.Add($"{count}. {activity}");
                }
            }
            else
            {
                ActivityList.Items.Add("No recent Activity found");
            }
        }

        //Close button 
        private void CloseButton_Click( object sender, RoutedEventArgs e )
        {
            Close();
        }
    }
}
