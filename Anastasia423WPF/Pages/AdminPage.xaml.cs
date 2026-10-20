using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Anastasia423WPF.Model;
using Anastasia423WPF.Windows;

namespace Anastasia423WPF.Pages
{
    public partial class AdminPage : Page
    {
        private User _currentUser;
        public List<Role> AllRoles { get; set; }

        public AdminPage(User user)
        {
            InitializeComponent();
            _currentUser = user;

            if (_currentUser.RoleID != 4)
            {
                UsersTab.Visibility = Visibility.Collapsed;
                DeleteManufacturerBtn.Visibility = Visibility.Collapsed;
                DeleteTypeBtn.Visibility = Visibility.Collapsed;
                gotoCatalog.Visibility = Visibility.Collapsed;
            }
            if(_currentUser.RoleID == 4)
            {
                goBack.Content = "Выйти из аккаунта";
            }
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                AllRoles = Core.Context.Role.ToList();
                ProductsGrid.ItemsSource = Core.Context.Product.ToList();
                ManufacturersGrid.ItemsSource = Core.Context.Manufacturer.ToList();
                TypesGrid.ItemsSource = Core.Context.Type.ToList();
                UsersGrid.ItemsSource = Core.Context.User.ToList();

                this.DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        // --- УПРАВЛЕНИЕ РОЛЯМИ И СТАТУСАМИ ---
        private void RoleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo == null || !combo.IsDropDownOpen) return;

            var selectedUser = combo.DataContext as User;
            var newRole = combo.SelectedItem as Role;

            if (selectedUser != null && newRole != null)
            {
                if (selectedUser.ID == _currentUser.ID && newRole.ID != 4)
                {
                    MessageBox.Show("Вы не можете лишить прав администратора самого себя!");
                    combo.SelectedItem = combo.Items.Cast<Role>().FirstOrDefault(r => r.ID == 4);
                    return;
                }
                selectedUser.RoleID = newRole.ID;
                Core.Context.SaveChanges();
                LoadData();
            }
        }

        private void ChangeUserStatus_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                if (selectedUser.ID == _currentUser.ID)
                {
                    MessageBox.Show("Вы не можете заморозить сами себя!");
                    return;
                }

                var window = new ChangeUserStatusWindow(selectedUser);
                if (window.ShowDialog() == true)
                {
                    LoadData(); // Перезагружаем таблицу, чтобы обновить текст статуса
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя из списка!");
            }
        }

        // --- ТОВАРЫ ---
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (new AddEditProductWindow(null).ShowDialog() == true) LoadData();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selected)
            {
                if (new AddEditProductWindow(selected).ShowDialog() == true) LoadData();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProductsGrid.SelectedItem is Product selected))
            {
                MessageBox.Show("Выберите товар из списка!");
                return;
            }
            if (!selected.IsActive)
            {
                if (MessageBox.Show($"Вернуть товар {selected.Name} в каталог?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
                selected.IsActive = true;
                Core.Context.SaveChanges();
                LoadData();
                return;
            }
            bool inOrders = Core.Context.Product_Order.Any(po => po.ProductID == selected.ID);

            if (inOrders)
            {
                if (MessageBox.Show($"Товар «{selected.Name}» есть в заказах, удалить его нельзя.\nСкрыть его из каталога?",
                    "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

                selected.IsActive = false;
            }
            else
            {
                if (MessageBox.Show($"Удалить товар «{selected.Name}» полностью?",
                    "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

                Core.Context.Product.Remove(selected);
            }

            Core.Context.SaveChanges();
            LoadData();
        }

        private void ProductsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateToggleButton();
        }

        private void UpdateToggleButton()
        {
            if (ProductsGrid.SelectedItem is Product selected && !selected.IsActive)
            {
                DeleteProductBtn.Content = "Вернуть в каталог";
                DeleteProductBtn.Background = new SolidColorBrush(Color.FromRgb(0x2E, 0x7D, 0x32)); // зелёный
            }
            else
            {
                DeleteProductBtn.Content = "Удалить";
                DeleteProductBtn.Background = new SolidColorBrush(Color.FromRgb(0x8B, 0x00, 0x00)); // тёмно-красный
            }
        }

        // --- ПРОИЗВОДИТЕЛИ ---
        private void AddManufacturer_Click(object sender, RoutedEventArgs e)
        {
            if (new DictionaryEditWindow(new Manufacturer()).ShowDialog() == true) LoadData();
        }

        private void EditManufacturer_Click(object sender, RoutedEventArgs e)
        {
            if (ManufacturersGrid.SelectedItem is Manufacturer selected)
            {
                if (new DictionaryEditWindow(selected).ShowDialog() == true) LoadData();
            }
        }

        private void DeleteManufacturer_Click(object sender, RoutedEventArgs e)
        {
            if (ManufacturersGrid.SelectedItem is Manufacturer selected)
            {
                if (Core.Context.Product.Any(p => p.ManufacturerID == selected.ID))
                {
                    MessageBox.Show("Нельзя удалить производителя, так как он привязан к товарам!");
                    return;
                }
                Core.Context.Manufacturer.Remove(selected);
                Core.Context.SaveChanges();
                LoadData();
            }
        }

        // --- ТИПЫ ---
        private void AddType_Click(object sender, RoutedEventArgs e)
        {
            if (new DictionaryEditWindow(new Type()).ShowDialog() == true) LoadData();
        }

        private void EditType_Click(object sender, RoutedEventArgs e)
        {
            if (TypesGrid.SelectedItem is Type selected)
            {
                if (new DictionaryEditWindow(selected).ShowDialog() == true) LoadData();
            }
        }

        private void DeleteType_Click(object sender, RoutedEventArgs e)
        {
            if (TypesGrid.SelectedItem is Type selected)
            {
                if (Core.Context.Product.Any(p => p.TypeID == selected.ID))
                {
                    MessageBox.Show("Нельзя удалить категорию, так как она привязана к товарам!");
                    return;
                }
                Core.Context.Type.Remove(selected);
                Core.Context.SaveChanges();
                LoadData();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.RoleID == 4)
            {
                // Для Администратора это полноценный выход из аккаунта
                if (!ConfirmationHelper.ConfirmLogout(Window.GetWindow(this))) return;
                NavigationService.Navigate(new AuthPage(_currentUser));
                // ОЧИЩАЕМ историю, чтобы кнопкой "Назад" нельзя было вернуться в админку
                while (NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
            else
            {
                // Для обычного менеджера это просто возврат на его страницу товаров/заказов
                if (!ConfirmationHelper.ConfirmBack(Window.GetWindow(this))) return;
                NavigationService.GoBack();
            }
        }

        private void gotoCatalog_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManagerPage(_currentUser));
        }
    }
}