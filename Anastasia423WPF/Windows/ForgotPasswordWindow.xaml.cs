using Anastasia423WPF.Pages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Anastasia423WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        User user;
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }
        private void ShowPassBoxes()
        {
            var UsInDb = Core.Context.User.FirstOrDefault(u => u.Login == LoginText.Text);
            if (UsInDb != null)
            {
                if (!String.IsNullOrWhiteSpace(PhoneNumberText.Text))
                {
                    if (PhoneNumberText.Text.Equals(UsInDb.Phone))
                    {
                        user = UsInDb;
                        VisibleBox();
                    }
                    else { MessageBox.Show("Неверно введен номер телефона! Попробуйте еще раз", "Ошибка ввода номера телефона", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            }
            else { MessageBox.Show("Пользователя нет в базе! Попробуйте еще раз", "Ошибка ввода логина", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        private void VisibleBox()
        {
            PasswordEnterText.Visibility = Visibility.Visible;
            NewPasswordText.Visibility = Visibility.Visible;
            ConfirmPasswordEnterText.Visibility = Visibility.Visible;
            NewPasswordConfirmText.Visibility = Visibility.Visible;
            SendRequest.Visibility = Visibility.Visible;
        }
        private void HideBox()
        {
            PasswordEnterText.Visibility = Visibility.Collapsed;
            NewPasswordText.Visibility = Visibility.Collapsed;
            ConfirmPasswordEnterText.Visibility = Visibility.Collapsed;
            NewPasswordConfirmText.Visibility = Visibility.Collapsed;
            SendRequest.Visibility = Visibility.Collapsed;
        }
        private void LoginText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(LoginText.Text) || String.IsNullOrEmpty(PhoneNumberText.Text))
            {
                HideBox();
            }
            ShowPassBoxes();
        }

        private void PhoneNumberText_LostFocus(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(PhoneNumberText.Text) || String.IsNullOrEmpty(LoginText.Text))
            {
                HideBox();
            }
            ShowPassBoxes();
        }

        private void SendRequest_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordText.Password.Equals(NewPasswordConfirmText.Password))
            {
                user.Password = PasswordHelper.HashPassword(NewPasswordConfirmText.Password);
                Core.Context.SaveChanges();
            }
            if((PasswordHelper.VerifyPassword(NewPasswordConfirmText.Password, user.Password)))
            {
                MessageBox.Show("Пароль успешно изменен!");
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
