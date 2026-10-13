using System;
using System.Windows;

namespace Anastasia423WPF.Windows
{
    public partial class ChangeUserStatusWindow : Window
    {
        private User _selectedUser;

        public ChangeUserStatusWindow(User user)
        {
            InitializeComponent();
            _selectedUser = user;
            StatusBox.Text = _selectedUser.Status; // Загружаем текущий статус
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StatusBox.Text))
            {
                MessageBox.Show("Введите статус!");
                return;
            }

            // Обновляем статус пользователя
            _selectedUser.Status = StatusBox.Text;

            try
            {
                Core.Context.SaveChanges();
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => this.DialogResult = false;
    }
}