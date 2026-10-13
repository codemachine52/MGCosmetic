using System;
using System.Windows;

namespace Anastasia423WPF.Windows
{
    public partial class ServiceDetailsWindow : Window
    {
        public ServiceDetailsWindow(Apointment selectedApt)
        {
            InitializeComponent();

            // Заполняем данные из переданного объекта
            ServiceNameTxt.Text = selectedApt.Service?.Name ?? "Услуга не указана";
            AptDateTxt.Text = $"Дата и время: {selectedApt.AppointmentDate:dd.MM.yyyy HH:mm}";

            // User1 обычно мастер, User - клиент (проверь связи в своей модели)
            MasterNameTxt.Text = selectedApt.User1?.FIO ?? "Не назначен";
            ClientNameTxt.Text = selectedApt.User?.FIO ?? "Не указан";

            PriceTxt.Text = $"{selectedApt.Service?.Price:N0} ₽";
            StatusTxt.Text = selectedApt.Status;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}