using System.Linq;
using Anastasia423WPF.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anastasia423WPF.Pages
{
    public partial class RegistrationPage : Page
    {
        private User us = new User();

        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmationHelper.ConfirmBack(Window.GetWindow(this))) return;
            NavigationService.Navigate(new AuthPage());
        }

        //Обработчики потери фокуса (Inline-валидация)
        private void LoginText_LostFocus(object sender, RoutedEventArgs e) => ValidateLogin();
        private void PassText_LostFocus(object sender, RoutedEventArgs e) => ValidatePassword();
        private void PassVerificText_LostFocus(object sender, RoutedEventArgs e) => ValidatePasswordMatch();
        private void FIOText_LostFocus(object sender, RoutedEventArgs e) => ValidateFIO();
        private void PhoneText_LostFocus(object sender, RoutedEventArgs e) => ValidatePhone();


        private void ShowError(TextBlock errorBlock, TextBox input, string message)
        {
            errorBlock.Text = message;
            errorBlock.Visibility = Visibility.Visible;
            input.BorderBrush = Brushes.Red;
            input.BorderThickness = new Thickness(2);
        }

        private void ShowError(TextBlock errorBlock, PasswordBox input, string message)
        {
            errorBlock.Text = message;
            errorBlock.Visibility = Visibility.Visible;
            input.BorderBrush = Brushes.Red;
            input.BorderThickness = new Thickness(2);
        }

        private void HideError(TextBlock errorBlock, Control input)
        {
            errorBlock.Visibility = Visibility.Collapsed;
            input.ClearValue(Control.BorderBrushProperty);
            input.ClearValue(Control.BorderThicknessProperty);
        }

        private bool ValidateLogin()
        {
            if (string.IsNullOrWhiteSpace(LoginText.Text))
            {
                ShowError(LoginError, LoginText, "Логин не может быть пустым");
                return false;
            }

            // Проверка на занятость логина в БД прямо при вводе
            var existingUser = Core.Context.User.FirstOrDefault(u => u.Login == LoginText.Text);
            if (existingUser != null)
            {
                ShowError(LoginError, LoginText, "Этот логин уже занят");
                return false;
            }

            HideError(LoginError, LoginText);
            return true;
        }

        private bool ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(PassText.Password))
            {
                ShowError(PassError, PassText, "Пароль не может быть пустым");
                return false;
            }
            if (PassText.Password.Length < 4)
            {
                ShowError(PassError, PassText, "Пароль слишком короткий (минимум 4 символа)");
                return false;
            }
            HideError(PassError, PassText);

            // Если подтверждение уже введено, проверяем совпадение заново
            if (!string.IsNullOrEmpty(PassVerificText.Password))
                ValidatePasswordMatch();

            return true;
        }

        private bool ValidatePasswordMatch()
        {
            if (PassText.Password != PassVerificText.Password)
            {
                ShowError(PassVerificError, PassVerificText, "Пароли не совпадают");
                return false;
            }
            HideError(PassVerificError, PassVerificText);
            return true;
        }

        private bool ValidateFIO()
        {
            if (string.IsNullOrWhiteSpace(FIOText.Text))
            {
                ShowError(FIOError, FIOText, "Укажите ваше ФИО");
                return false;
            }
            HideError(FIOError, FIOText);
            return true;
        }

        private bool ValidatePhone()
        {
            string cleanPhone = PhoneText.Text.Trim();
            if (string.IsNullOrWhiteSpace(cleanPhone))
            {
                ShowError(PhoneError, PhoneText, "Укажите номер телефона");
                return false;
            }

            bool isPhoneValid = (cleanPhone.StartsWith("+") && cleanPhone.Length == 12) ||
                                (!cleanPhone.StartsWith("+") && cleanPhone.Length == 11);

            if (!isPhoneValid)
            {
                ShowError(PhoneError, PhoneText, "Формат: 11 цифр или 12 с '+' (напр. +79991234567)");
                return false;
            }

            HideError(PhoneError, PhoneText);
            return true;
        }

        private void RegistrUser_Click(object sender, RoutedEventArgs e)
        {
            // все проверки разом. Используем одинарное &, чтобы выполнить ВСЕ методы 
            // (иначе последующие методы не вызовутся, если первый вернул false)
            bool isValid = ValidateLogin() & ValidatePassword() & ValidatePasswordMatch() & ValidateFIO() & ValidatePhone();

            if (isValid)
            {
                us.Login = LoginText.Text;
                us.Password = PasswordHelper.HashPassword(PassText.Password);
                us.FIO = FIOText.Text;
                us.Phone = PhoneText.Text;

                // обязательные для БД
                us.RoleID = 1;         // 1 = Клиент (из таблицы Role)
                us.Rating = 0.0;       
                us.Status = "Active";  

                try
                {
                    Core.Context.User.Add(us);
                    Core.Context.SaveChanges();

                    MessageBox.Show("Вы успешно зарегистрированы!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new Catalog(us));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении в базу данных: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, исправьте ошибки в подсвеченных полях.", "Ошибка заполнения", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}