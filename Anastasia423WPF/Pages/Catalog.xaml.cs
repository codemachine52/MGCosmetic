using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Anastasia423WPF.Model;

namespace Anastasia423WPF.Pages
{
    
    public partial class Catalog : Page
    {
        public User user { get; set; }

        public Catalog()
        {
            InitializeComponent();
            LoadFilters();
            UpdateProducts();
            if (user != null && (user.RoleID == 3 || user.RoleID == 4))
            {
                AdminPanelBtn.Visibility = Visibility.Visible;
            }
        }

        public Catalog(User us) : this()
        {
            user = us;
            this.DataContext = user;


            // Если пользователь авторизован:
            if (user != null)
            {
                AuthUser.Content = "👤";
                AuthUser.Width = 80;
                if (user.RoleID == 1) CartBtn.Visibility = Visibility.Visible; // Показываем кнопку корзины

                if (user.RoleID == 3 || user.RoleID == 4)
                {
                    AdminPanelBtn.Visibility = Visibility.Visible;
                }
            }
        }

        // Загрузка данных в выпадающие списки
        private void LoadFilters()
        {
            // Сортировка
            SortCombo.Items.Add("Без сортировки");
            SortCombo.Items.Add("По оценке (Сначала высокие)");
            SortCombo.Items.Add("По оценке (Сначала низкие)");
            SortCombo.SelectedIndex = 0;

            // Типы товаров
            var types = Core.Context.Type.ToList();
            types.Insert(0, new Type { ID = 0, Name = "Все типы" });
            TypeCombo.ItemsSource = types;
            TypeCombo.SelectedIndex = 0;

            // Производители
            var manufs = Core.Context.Manufacturer.ToList();
            manufs.Insert(0, new Manufacturer { ID = 0, Name = "Все производители" });
            ManufCombo.ItemsSource = manufs;
            ManufCombo.SelectedIndex = 0;
        }

        // Универсальный метод обновления списка (Поиск + Фильтры + Сортировка)
        private void UpdateProducts()
        {
            // только активные товары
            var currentProducts = Core.Context.Product.Where(p => p.IsActive).ToList();

            // 1. Поиск по названию
            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                currentProducts = currentProducts.Where(p => p.Name.ToLower().Contains(SearchBox.Text.ToLower())).ToList();
            }

            // 2. Фильтрация по типу
            if (TypeCombo.SelectedIndex > 0)
            {
                var selectedType = TypeCombo.SelectedItem as Type;
                currentProducts = currentProducts.Where(p => p.TypeID == selectedType.ID).ToList();
            }

            // 3. Фильтрация по производителю
            if (ManufCombo.SelectedIndex > 0)
            {
                var selectedManuf = ManufCombo.SelectedItem as Manufacturer;
                currentProducts = currentProducts.Where(p => p.ManufacturerID == selectedManuf.ID).ToList();
            }

            // 4. Сортировка по оценке
            if (SortCombo.SelectedIndex == 1) // Высокие
                currentProducts = currentProducts.OrderByDescending(p => p.Rating).ToList();
            else if (SortCombo.SelectedIndex == 2) // Низкие
                currentProducts = currentProducts.OrderBy(p => p.Rating).ToList();

            ProductsList.ItemsSource = currentProducts;
        }

        // Обработчик изменения любых фильтров (Search, Comboboxes)
        private void Filter_Changed(object sender, SelectionChangedEventArgs e) => UpdateProducts();
        private void Filter_Changed(object sender, TextChangedEventArgs e) => UpdateProducts();

        // Добавление в корзину (по кнопке на товаре)
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (user == null)
            {
                MessageBox.Show("Для добавления товара в корзину необходимо авторизоваться!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.Navigate(new AuthPage());
                return;
            }

            // Получаем товар, на кнопку которого нажали
            var button = sender as Button;
            var product = button.DataContext as Product;

            ShoppingCart.Add(product);
            MessageBox.Show($"Товар '{product.Name}' добавлен в корзину!", "Успех");
        }

        // Открытие карточки товара в НОВОМ ОКНЕ по двойному клику
        private void ProductsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductsList.SelectedItem is Product selectedProduct)
            {
                // Создаем и открываем наше новое окно
                var detailsWindow = new Windows.ProductDetailsWindow(selectedProduct, user);
                detailsWindow.ShowDialog(); // ShowDialog заблокирует каталог, пока не закроют детали
            }
        }

        private void AuthUser_Click(object sender, RoutedEventArgs e)
        {
            if (user == null)
                NavigationService.Navigate(new AuthPage());
            else
                NavigationService.Navigate(new ProfilePage(user));
        }

        private void CartBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage(user));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (user != null)
            {
                if(user.RoleID == 1) NavigationService.Navigate(new MainHubPage(user));
                else
                {
                    NavigationService.Navigate(new StartPage(user));
                }
            }
            else NavigationService.Navigate(new StartPage());
        }

        private void AdminPanelBtn_Click(object sender, RoutedEventArgs e)
        {
            // Переходим на страницу админки, передавая текущего пользователя
            NavigationService.Navigate(new AdminPage(user));
        }
    }
}

//история заказов: просмотр каждого заказа
//запись на услуги пустая стр почему-то
