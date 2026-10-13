using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Anastasia423WPF.Windows;

namespace Anastasia423WPF.Pages
{
    public partial class ManagerPage : Page
    {
        User _currentUser;

        public ManagerPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadData(); // Первичная загрузка при открытии
        }

        /// <summary>
        /// Универсальный метод обновления всех данных на странице
        /// </summary>
        private void LoadData()
        {
            // Обновляем заказы товаров
            OrdersGrid.ItemsSource = Core.Context.Order.ToList();

            // Обновляем записи на услуги
            AppointmentsGrid.ItemsSource = Core.Context.Apointment.ToList();
        }

        // --- ЛОГИКА ЗАКАЗОВ ---

        private void IssueOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                if (selectedOrder.Status == "Выдан")
                {
                    MessageBox.Show("Заказ уже выдан!");
                    return;
                }

                // 1. Ищем самого пользователя (клиента) в таблице User по ClientID из заказа
                var client = Core.Context.User.FirstOrDefault(u => u.ID == selectedOrder.ClientID);

                // 2. Проверяем статус найденного клиента
                if (client != null && client.Status == "Freeze")
                {
                    MessageBox.Show("Невозможно выдать заказ: аккаунт клиента заморожен!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Прерываем выполнение
                }

                // 3. Если клиент не заморожен, выдаем заказ
                selectedOrder.Status = "Выдан";
                // selectedOrder.DeliveryDate = DateTime.Now; 

                Core.Context.SaveChanges();
                MessageBox.Show("Заказ успешно выдан клиенту!");
                LoadData(); // Обновляем списки
            }
            else
            {
                MessageBox.Show("Выберите заказ из списка!");
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                OrderDetailsWindow details = new OrderDetailsWindow(selectedOrder);
                details.ShowDialog();
            }
        }

        // --- ЛОГИКА ЗАПИСЕЙ ---

        // ДОБАВЛЕНИЕ
        private void AddApt_Click(object sender, RoutedEventArgs e)
        {
            var editWin = new AppointmentEditWindow(); // пустой конструктор

            if (editWin.ShowDialog() == true)
            {
                Apointment newApt = new Apointment
                {
                    AppointmentDate = editWin.SelectedDateTime,
                    ClientID = editWin.SelectedClient.ID,
                    MasterID = editWin.SelectedMaster.ID,
                    ServiceID = editWin.SelectedService.ID,
                    PaymentWay = editWin.SelectedPayment,
                    Status = "Запланирована"
                };

                try
                {
                    Core.Context.Apointment.Add(newApt);
                    Core.Context.SaveChanges();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        // РЕДАКТИРОВАНИЕ
        private void EditApt_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsGrid.SelectedItem is Apointment selectedApt)
            {
                // Передаем весь объект записи целиком в конструктор
                var editWin = new AppointmentEditWindow(selectedApt);

                if (editWin.ShowDialog() == true)
                {
                    try
                    {
                        // Обновляем все поля из окна
                        selectedApt.AppointmentDate = editWin.SelectedDateTime;
                        selectedApt.ClientID = editWin.SelectedClient.ID;
                        selectedApt.MasterID = editWin.SelectedMaster.ID;
                        selectedApt.ServiceID = editWin.SelectedService.ID;
                        selectedApt.PaymentWay = editWin.SelectedPayment;

                        Core.Context.SaveChanges();
                        MessageBox.Show($"Запись №{selectedApt.ID} успешно обновлена!");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении: " + ex.Message);
                    }
                }
            }
        }

        private void CancelApt_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsGrid.SelectedItem is Apointment selectedApt)
            {
                selectedApt.Status = "Отменен";
                Core.Context.SaveChanges();
                MessageBox.Show("Запись отменена");
                LoadData();
            }
        }

        private void DeleteApt_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsGrid.SelectedItem is Apointment selectedApt)
            {
                if (MessageBox.Show("Удалить запись безвозвратно?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Core.Context.Apointment.Remove(selectedApt);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Запись удалена");
                    LoadData();
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminPage(_currentUser));
        }

        private void ExitAccButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage(_currentUser));
        }
    }
}