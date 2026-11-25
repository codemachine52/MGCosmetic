using System;
using Anastasia423WPF.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Anastasia423WPF.Windows;
using System.Data.Entity;

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
            if (_currentUser.RoleID == 1)
            {
                OrderHistoryText.Visibility = Visibility.Visible;
                OrdersList.Visibility = Visibility.Visible;
                AppointmentText.Visibility = Visibility.Visible;    
                AppointmentsList.Visibility = Visibility.Visible;
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
            if (_currentUser.RoleID == 2) 
                {
                HistoryAppointmentText.Visibility = Visibility.Visible;
                HistoryAppointmentList.Visibility = Visibility.Visible;
                ButtonBack.Content = "Вернуться к записям";
                //история выполненных услуг мастера
                HistoryAppointmentList.ItemsSource = Core.Context.Apointment.Where(a => a.MasterID == _currentUser.ID && a.Status.Equals("Выполнено")).ToList();
                }
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
            if (!ConfirmationHelper.ConfirmLogout(Window.GetWindow(this))) return;
            NavigationService.Navigate(new AuthPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmationHelper.ConfirmBack(Window.GetWindow(this))) return;
            if (_currentUser.RoleID == 1)
            { 
            NavigationService.Navigate(new Catalog(_currentUser));
            }
            if (_currentUser.RoleID == 2)
            {
                NavigationService.Navigate(new MasterCabinetPage(_currentUser));
            }
        }
    }
}