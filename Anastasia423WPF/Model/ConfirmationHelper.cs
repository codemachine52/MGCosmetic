using System.Windows;

namespace Anastasia423WPF.Model
{
    /// <summary>
    /// Единое место для диалогов подтверждения («Вы уверены?»), чтобы тексты
    /// и поведение не дублировались по страницам.
    /// По умолчанию выбран вариант «Нет», чтобы случайное нажатие Enter ничего не подтверждало.
    /// </summary>
    public static class ConfirmationHelper
    {
        public static bool Confirm(string message, string title, Window owner = null)
        {
            MessageBoxResult result = owner != null
                ? MessageBox.Show(owner, message, title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
                : MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            return result == MessageBoxResult.Yes;
        }

        /// <summary>Подтверждение выхода из программы.</summary>
        public static bool ConfirmExit(Window owner = null)
        {
            return Confirm("Вы уверены, что хотите выйти из программы?", "Подтверждение выхода", owner);
        }

        /// <summary>Подтверждение выхода из учётной записи.</summary>
        public static bool ConfirmLogout(Window owner = null)
        {
            return Confirm("Вы уверены, что хотите выйти из аккаунта?", "Подтверждение выхода", owner);
        }

        /// <summary>Подтверждение возврата назад. Можно передать свой текст вопроса.</summary>
        public static bool ConfirmBack(Window owner = null, string message = null)
        {
            return Confirm(message ?? "Вы уверены, что хотите вернуться назад?", "Подтверждение", owner);
        }
    }
}
