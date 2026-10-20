using Anastasia423WPF.Windows;
using Anastasia423WPF.Model;
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

namespace Anastasia423WPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public User user { get; set; }
        public AuthPage()
        {
            InitializeComponent();
        }

        public AuthPage(User us) : this()
        {
            if (us != null)
            {
                user = us;
                LoginText.Text += user.Login;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Catalog());
        }

        private void Registr_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            // После 3 неверных паролей подряд вход возможен только после прохождения капчи
            if (LoginAttemptTracker.IsCaptchaRequired(LoginText.Text) && !PassCaptcha())
            {
                return;
            }

            if(AuthUser(LoginText.Text, PassText.Password))
            {
                LoginAttemptTracker.Reset(LoginText.Text);

                if (user.Status == "Freeze")
                {
                    MessageBox.Show("Ваш аккаунт заблокирован администратором.", "Ошибка доступа");
                    return; // Прерываем вход
                }
                // Если статус норм, пускаем дальше...
                MessageBox.Show("Успешный вход!", "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
                if (user.RoleID == 2) // Мастер
                {
                    NavigationService.Navigate(new MasterCabinetPage(user));
                }
                else if (user.RoleID == 4)
                {
                    NavigationService.Navigate(new AdminPage(user));
                }// manager
                else if (user.RoleID == 3) NavigationService.Navigate(new ManagerPage(user));
                else
                {
                    NavigationService.Navigate(new MainHubPage(user));
                }
            }
            else
            {
                // Третья неверная попытка подряд — сразу показываем капчу
                if (LoginAttemptTracker.IsCaptchaRequired(LoginText.Text))
                {
                    PassCaptcha();
                }
            }
        }

        /// <summary>
        /// Показывает окно с примером. Возвращает true, если пример решён верно
        /// (счётчик неудачных попыток при этом сбрасывается).
        /// </summary>
        private bool PassCaptcha()
        {
            var captcha = new CaptchaWindow { Owner = Window.GetWindow(this) };

            if (captcha.ShowDialog() == true)
            {
                LoginAttemptTracker.Reset(LoginText.Text);
                return true;
            }

            return false;
        }

        public bool AuthUser(string login, string password)
        {
            var UsInDB = Core.Context.User.Where(u => u.Login == login).FirstOrDefault();
            if (UsInDB != null)
            {
                if (!string.IsNullOrEmpty(password))
                {
                    if (PasswordHelper.VerifyPassword(password, UsInDB.Password))
                    {
                        user = UsInDB;
                        return true;
                    }
                    else
                    {
                        // Неверный пароль — запоминаем попытку (после 3-й понадобится капча)
                        LoginAttemptTracker.RegisterFailure(login);
                        MessageBox.Show("Введен неверный пароль!", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Введен неверный пароль!", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Ошибка! Пользователь не найден в базе данных!", "Не найден в базе данных", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void Forgot_Click(object sender, RoutedEventArgs e)
        {
            var forg = new ForgotPasswordWindow();
            if(forg.ShowDialog() == true)
            {
                user.Password = PasswordHelper.HashPassword(forg.NewPasswordText.Password);
            }
        }
    }
}
