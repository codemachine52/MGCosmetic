using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
                    LoadData();
                    return;
                }

                selectedUser.RoleID = newRole.ID;
                Core.Context.SaveChanges();
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
            if (ProductsGrid.SelectedItem is Product selected)
            {
                if (MessageBox.Show($"Удалить товар {selected.Name}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Core.Context.Product.Remove(selected);
                    Core.Context.SaveChanges();
                    LoadData();
                }
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
                try
                {
                    Core.Context.Manufacturer.Remove(selected);
                    Core.Context.SaveChanges();
                    LoadData();
                }
                catch { MessageBox.Show("Нельзя удалить производителя, так как он привязан к товарам!"); }
            }
        }

        // --- ТИПЫ ---
        private void AddType_Click(object sender, RoutedEventArgs e)
        {
            if (new DictionaryEditWindow(new Anastasia423WPF.Type()).ShowDialog() == true) LoadData();
        }

        private void EditType_Click(object sender, RoutedEventArgs e)
        {
            if (TypesGrid.SelectedItem is Anastasia423WPF.Type selected)
            {
                if (new DictionaryEditWindow(selected).ShowDialog() == true) LoadData();
            }
        }

        private void DeleteType_Click(object sender, RoutedEventArgs e)
        {
            if (TypesGrid.SelectedItem is Anastasia423WPF.Type selected)
            {
                try
                {
                    Core.Context.Type.Remove(selected);
                    Core.Context.SaveChanges();
                    LoadData();
                }
                catch { MessageBox.Show("Нельзя удалить тип, так как он используется в записях!"); }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}