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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Anastasia423WPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        User _user;
        public StartPage()
        {
            InitializeComponent();
        }

        public StartPage(User user) : this()
        {
            _user = user;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null)
            {
                NavigationService.Navigate(new AuthPage());
            }
            else
            {
                if (_user.RoleID != 1) NavigationService.Navigate(new Catalog(_user));
                else { NavigationService.Navigate(new MainHubPage(_user)); }
            }
        }
    }
}
