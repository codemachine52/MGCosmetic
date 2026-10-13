using System.Windows;
using Anastasia423WPF.Model;

namespace Anastasia423WPF.Windows
{
    public partial class ProductDetailsWindow : Window
    {
        public Product CurrentProduct { get; set; }
        public User CurrentUser { get; set; }

        public ProductDetailsWindow(Product product, User user)
        {
            InitializeComponent();
            CurrentProduct = product;
            CurrentUser = user;

            // Устанавливаем данные для привязки в XAML
            this.DataContext = CurrentProduct;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddToCartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("Авторизуйтесь для покупки!", "Внимание");
                return;
            }

            // Логика добавления (можно сделать через статический класс корзины)
            ShoppingCart.Add(CurrentProduct);
            MessageBox.Show($"{CurrentProduct.Name} добавлен в корзину!");
        }
    }
}