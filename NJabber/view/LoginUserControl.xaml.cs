using System.Windows.Controls;
using MVCWork;
using NJabber.Notifications;
using NJabber.Properties;

namespace NJabber.view
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginUserControl : UserControl, IDisplay
    {
        public LoginUserControl()
        {
            InitializeComponent();
            userNameTextbox.Text = Settings.Default.UserName;
            passwordBox.Password = Settings.Default.Password;



        }

        private void loginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var loginNotification = new LoginNotification
                                       {
                                           UserName = userNameTextbox.Text,
                                           Password = passwordBox.Password
                                       };
            Settings.Default.UserName = userNameTextbox.Text;
            Settings.Default.Password = passwordBox.Password;
            Settings.Default.Save();
            Facade.Intance.SendNotification(ApplicationNotifications.LoginPressed, loginNotification);

        }
    }
}
