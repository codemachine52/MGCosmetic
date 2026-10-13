using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Anastasia423WPF.Pages
{
    public partial class ServiceBookingPage : Page
    {
        private Service _selectedService;
        private User _currentUser;

        public ServiceBookingPage(Service service, User user)
        {
            InitializeComponent();
            _selectedService = service;
            _currentUser = user;

            // Отображаем название услуги
            TxtServiceName.Text = _selectedService.Name;

            // Загружаем только мастеров (Роль ID = 2)
            ComboMasters.ItemsSource = Core.Context.User.Where(u => u.RoleID == 2 && u.Status != "Freeze").ToList();

            DatePick.DisplayDateStart = DateTime.Today;
        }

        private void ConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (ComboMasters.SelectedItem == null || DatePick.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите мастера и дату записи!");
                return;
            }

            try
            {
                // Создаем новую запись в БД
                var appointment = new Apointment
                {
                    ServiceID = _selectedService.ID,
                    ClientID = _currentUser.ID,
                    MasterID = (int)ComboMasters.SelectedValue,
                    AppointmentDate = (DateTime)DatePick.SelectedDate,
                    Status = "Ожидание", // Начальный статус
                    PaymentWay = "На месте",
                    Comment = TxtComment.Text
                };

                Core.Context.Apointment.Add(appointment);
                Core.Context.SaveChanges();

                MessageBox.Show("Вы успешно записаны на услугу!", "Успех");
                NavigationService.GoBack(); // Возвращаемся в каталог
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}