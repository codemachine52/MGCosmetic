using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Anastasia423WPF.Pages
{
    public partial class MasterCabinetPage : Page
    {
        private User _master;

        public MasterCabinetPage(User master)
        {
            InitializeComponent();
            _master = master;

            LoadServiceTypes();
            RefreshData();
        }

        private void LoadServiceTypes()
        {
            // Получаем только те типы услуг, которые есть в записях у этого мастера
            var types = Core.Context.Apointment
                .Where(a => a.MasterID == _master.ID)
                .Select(a => a.Service)
                .Distinct()
                .ToList();

            ServiceTypeCombo.ItemsSource = types;
        }

        private void RefreshData()
        {
            var query = Core.Context.Apointment
                .Where(a => a.MasterID == _master.ID && a.Status != "Выполнено" && a.Status != "Отменен");

            // Если выбран конкретный тип услуги в ComboBox
            if (ServiceTypeCombo.SelectedItem is Service selectedService)
            {
                query = query.Where(a => a.ServiceID == selectedService.ID);
            }

            MasterAptList.ItemsSource = query.OrderBy(a => a.AppointmentDate).ToList();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext is Apointment apt)
            {
                apt.Status = "Выполнено";
                Core.Context.SaveChanges();
                MessageBox.Show("Услуга успешно оказана!");
                RefreshData();
            }
        }

        private void ServiceTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshData(); // Перегружаем список при выборе типа
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            ServiceTypeCombo.SelectedItem = null;
            RefreshData();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}