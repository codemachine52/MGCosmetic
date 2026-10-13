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
using Anastasia423WPF.Model;

namespace Anastasia423WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        public OrderDetailsWindow(Order selectedOrder)
        {
            InitializeComponent();

            // 1. Получаем список товаров именно для ЭТОГО заказа из базы данных
            var orderItems = Core.Context.Product_Order
    .Where(po => po.OrderID == selectedOrder.ID)
    .ToList();

            // 2. Считаем итоговую сумму (приводим double к decimal внутри суммы)
            decimal total = orderItems.Sum(po =>
            {
                decimal price = (decimal)po.Product.Price;
                decimal discount = (decimal)(po.Product.Discount); // Если Discount не nullable, ?? не нужен
                decimal count = (decimal)po.CountProd;

                return (price * (1 - discount / 100)) * count;
            });
            // 3. Выводим данные в интерфейс
            OrderNumberTxt.Text = $"Заказ №{selectedOrder.ID}";
            OrderDateTxt.Text = $"Дата оформления: {selectedOrder.OrderDate:dd.MM.yyyy}";
            TotalPriceTxt.Text = $"Сумма заказа: {total:N0} ₽";

            // 4. Привязываем данные к списку
            OrderItemsList.ItemsSource = orderItems;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
//обязательно список не только заказов но и записей в профиле
//выдавать заказ может менеджер и соответственно меняется статус заказа, а также ставится дата получения после выдачи менеджером заказа
//кнопки регулировки количества товара в корзине и удаление из корзины
//при нажатии на кнопку ЗАКАЗАТЬ в корзине нужно создать новое окно где будет выбираться тип оплаты, удобный день получения заказа (не больше 7 дней)
//мастер может открывать каждую запись и ставить метку о выполнении, а также выбирать типы своих услуг
