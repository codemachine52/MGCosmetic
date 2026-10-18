using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Anastasia423WPF.Windows
{
    /// <summary>
    /// Простая «капча» для авторизации: случайный арифметический пример,
    /// ответ на который нужно ввести, чтобы продолжить вход.
    /// ShowDialog() возвращает true, если пример решён верно.
    /// </summary>
    public partial class CaptchaWindow : Window
    {
        // Один общий генератор: несколько Random, созданных подряд, могут выдать одинаковые числа.
        private static readonly Random Rnd = new Random();

        private int _answer;

        public CaptchaWindow()
        {
            InitializeComponent();
            GenerateExample();
            Loaded += (s, e) => AnswerBox.Focus();
        }

        /// <summary>Создаёт новый случайный пример (сложение, вычитание или умножение).</summary>
        private void GenerateExample()
        {
            int a;
            int b;
            string sign;

            switch (Rnd.Next(3))
            {
                case 0:
                    a = Rnd.Next(5, 50);
                    b = Rnd.Next(5, 50);
                    _answer = a + b;
                    sign = "+";
                    break;
                case 1:
                    a = Rnd.Next(20, 80);
                    b = Rnd.Next(5, a);      // b < a, поэтому результат всегда положительный
                    _answer = a - b;
                    sign = "\u2212";         // знак «−»
                    break;
                default:
                    a = Rnd.Next(2, 10);
                    b = Rnd.Next(2, 10);
                    _answer = a * b;
                    sign = "\u00D7";         // знак «×»
                    break;
            }

            ExampleText.Text = a + " " + sign + " " + b + " = ?";
            AnswerBox.Clear();
        }

        // Разрешаем вводить только цифры.
        private void AnswerBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string text = AnswerBox.Text.Trim();

            if (text.Length == 0)
            {
                ShowError("Введите ответ на пример.");
                AnswerBox.Focus();
                return;
            }

            int value;
            if (int.TryParse(text, out value) && value == _answer)
            {
                DialogResult = true;
                return;
            }

            // Неверный ответ — показываем новый пример, чтобы нельзя было подбирать ответ.
            ShowError("Неверный ответ. Попробуйте решить новый пример.");
            GenerateExample();
            AnswerBox.Focus();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
