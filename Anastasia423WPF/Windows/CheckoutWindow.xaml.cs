using System;
using System.Windows;
using System.Windows.Controls;

namespace Anastasia423WPF.Windows
{
    public partial class CheckoutWindow : Window
    {
        // Свойства для получения данных в окне корзины после закрытия этого окна
        public DateTime SelectedDate { get; private set; }
        public string SelectedPayment { get; private set; }

        public CheckoutWindow(decimal totalSum)
        {
            InitializeComponent();

            // Выводим сумму, которую передали из корзины
            TotalSumTxt.Text = $"{totalSum:N0} ₽";

            // Ограничение календаря согласно ТЗ (не более 7 дней)
            OrderDatePicker.DisplayDateStart = DateTime.Now; // Нельзя выбрать прошлое
            OrderDatePicker.DisplayDateEnd = DateTime.Now.AddDays(7); // Не дальше недели
            OrderDatePicker.SelectedDate = DateTime.Now; // По умолчанию сегодня
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на заполнение
            if (OrderDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите дату получения заказа.");
                return;
            }

            // Сохраняем выбор в свойства
            SelectedDate = OrderDatePicker.SelectedDate.Value;
            SelectedPayment = (PaymentCombo.SelectedItem as ComboBoxItem).Content.ToString();

            // Ставим результат True, чтобы корзина поняла, что нажали "Подтвердить"
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}