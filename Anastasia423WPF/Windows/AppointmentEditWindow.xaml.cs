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

namespace Anastasia423WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для AppointmentEditWindow.xaml
    /// </summary>
    public partial class AppointmentEditWindow : Window
    {
        // Свойства для получения данных из окна
        public DateTime SelectedDateTime => AptDatePicker.SelectedDate ?? DateTime.Now;
        public User SelectedClient => ClientCombo.SelectedItem as User;
        public User SelectedMaster => MasterCombo.SelectedItem as User;
        public Service SelectedService => ServiceCombo.SelectedItem as Service;
        public string SelectedPayment => (PaymentCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

        // Конструктор теперь принимает необязательный параметр - существующую запись
        public AppointmentEditWindow(Apointment existingApt = null)
        {
            InitializeComponent();

            AptDatePicker.DisplayDateStart = DateTime.Today;
            
            // Загружаем списки для выбора
            ClientCombo.ItemsSource = Core.Context.User.Where(u => u.RoleID == 1 && u.Status != "Freeze").ToList();
            MasterCombo.ItemsSource = Core.Context.User.Where(u => u.RoleID == 2 && u.Status != "Freeze").ToList();
            ServiceCombo.ItemsSource = Core.Context.Service.ToList();

            if (existingApt != null)
            {
                // Если мы редактируем, подставляем старые значения
                AptDatePicker.SelectedDate = existingApt.AppointmentDate;
                
                ClientCombo.SelectedItem = existingApt.User;   // Связь Client
                MasterCombo.SelectedItem = existingApt.User1;  // Связь Master
                ServiceCombo.SelectedItem = existingApt.Service;

                // Установка типа оплаты (если текст совпадает с Content в ComboBoxItem)
                foreach (ComboBoxItem item in PaymentCombo.Items)
                {
                    if (item.Content.ToString() == existingApt.PaymentWay)
                        PaymentCombo.SelectedItem = item;
                }
            }
            else
            {
                AptDatePicker.SelectedDate = DateTime.Today;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (AptDatePicker.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Нельзя создать запись на прошедшую дату!");
                return;
            }

            // Проверка остальных полей
            if (ClientCombo.SelectedItem == null || MasterCombo.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            this.DialogResult = true;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}