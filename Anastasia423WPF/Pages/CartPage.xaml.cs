using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Anastasia423WPF.Model;
using Anastasia423WPF.Windows;

namespace Anastasia423WPF.Pages
{
    public partial class CartPage : Page
    {
        private User _currentUser;

        public CartPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            RefreshCart();
        }

        private void RefreshCart()
        {
            // Перепривязываем список для обновления UI
            CartList.ItemsSource = null;
            CartList.ItemsSource = ShoppingCart.Items;

            // Считаем сумму с учетом скидки и КОЛИЧЕСТВА (наш CountProd из partial класса)
            decimal total = ShoppingCart.Items.Sum(p => p.Price * (1 - (decimal)p.Discount / 100) * p.CountProd);
            TotalSumText.Text = $"{total:N0} ₽";
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext is Product product)
            {
                if (product.CountProd >= product.Quantity)
                {
                    MessageBox.Show($"На складе доступно только {product.Quantity} шт.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                product.CountProd++;
                RefreshCart();
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext is Product product && product.CountProd > 1)
            {
                product.CountProd--;
                RefreshCart();
            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext is Product product)
            {
                product.CountProd = 1;
                ShoppingCart.Items.Remove(product);
                RefreshCart();
            }
        }

        private void Order_Click(object sender, RoutedEventArgs e)
        {
            if (ShoppingCart.Items.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Берём актуальные остатки из БД и проверяем, что товара хватает
            foreach (var item in ShoppingCart.Items)
            {
                Core.Context.Entry(item).Reload();
                if (item.CountProd > item.Quantity)
                {
                    MessageBox.Show($"Товара '{item.Name}' недостаточно на складе. В наличии: {item.Quantity} шт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Итоговая сумма для окна
            decimal total = ShoppingCart.Items.Sum(p => p.Price * (1 - (decimal)p.Discount / 100) * p.CountProd);

            CheckoutWindow checkout = new CheckoutWindow(total);
            checkout.Owner = Window.GetWindow(this);

            if (checkout.ShowDialog() == true)
            {
                try
                {
                    Order newOrder = new Order
                    {
                        ClientID = _currentUser.ID,
                        OrderDate = DateTime.Now,
                        DelieveryDate = checkout.SelectedDate,
                        PaymentWay = checkout.SelectedPayment,
                        Status = "Новый",
                        ReceiveDate = checkout.SelectedDate
                    };

                    // Состав заказа и списание со склада сохраняются вместе с заказом одной транзакцией
                    foreach (var item in ShoppingCart.Items)
                    {
                        newOrder.Product_Order.Add(new Product_Order
                        {
                            ProductID = item.ID,
                            CountProd = item.CountProd
                        });
                        item.Quantity -= item.CountProd;
                    }

                    Core.Context.Order.Add(newOrder);
                    Core.Context.SaveChanges();

                    MessageBox.Show($"Заказ №{newOrder.ID} успешно оформлен!");
                    foreach (var item in ShoppingCart.Items)
                    {
                        item.CountProd = 1;
                    }
                    ShoppingCart.Items.Clear();
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmationHelper.ConfirmBack(Window.GetWindow(this))) return;
            NavigationService.GoBack();
        }
    }
}