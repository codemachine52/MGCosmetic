using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Anastasia423WPF.Pages
{
    public partial class MainHubPage : Page
    {
        private User _currentUser;

        public MainHubPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        // Переход в магазин товаров
        private void GoToShop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Catalog(_currentUser));
        }

        // Переход в каталог услуг
        private void GoToServices_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServiceCatalogPage(_currentUser));
        }

        // Кнопка выхода (можно добавить в угол страницы)
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}