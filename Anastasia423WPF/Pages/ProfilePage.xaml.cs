using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Anastasia423WPF.Windows;

namespace Anastasia423WPF.Pages
{
    public partial class ProfilePage : Page
    {
        private User _currentUser;

        public ProfilePage(User user)
        {
            InitializeComponent();
            _currentUser = user;

            // Привязываем данные пользователя к шапке (для FIO, RoleName и т.д.)
            DataContext = _currentUser;

            LoadData();
        }

        private void LoadData()
        {
            // История заказов товаров
            OrdersList.ItemsSource = Core.Context.Order
                .Where(o => o.ClientID == _currentUser.ID)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            // История записей на услуги
            AppointmentsList.ItemsSource = Core.Context.Apointment
                .Where(a => a.ClientID == _currentUser.ID)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        private void OrdersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrdersList.SelectedItem is Order selectedOrder)
            {
                OrderDetailsWindow details = new OrderDetailsWindow(selectedOrder);
                details.ShowDialog();
            }
        }

        private void AppointmentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AppointmentsList.SelectedItem is Apointment selectedApt)
            {
                ServiceDetailsWindow details = new ServiceDetailsWindow(selectedApt);
                details.ShowDialog();
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Catalog(_currentUser));
        }
    }
}