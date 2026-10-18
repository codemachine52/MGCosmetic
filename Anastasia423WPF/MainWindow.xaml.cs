using Anastasia423WPF.Pages;
using Anastasia423WPF.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Anastasia423WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Width = 900;
            this.Height = 708;

            // Переключатель темы: подпись кнопки зависит от текущей темы
            UpdateThemeButton();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            Closed += (s, e) => ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;

            // Системные команды «назад» (Alt+Left, Backspace, боковая кнопка мыши) тоже спрашивают подтверждение
            AddHandler(CommandManager.PreviewExecutedEvent, new ExecutedRoutedEventHandler(OnPreviewCommandExecuted));
        }

        private void ThemeManager_ThemeChanged(object sender, EventArgs e)
        {
            UpdateThemeButton();
        }

        private void UpdateThemeButton()
        {
            ThemeToggleButton.Content = ThemeManager.CurrentTheme == AppTheme.Light
                ? "\U0001F319 Тёмная тема"
                : "\u2600 Светлая тема";
        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.Toggle();
        }

        /// <summary>Подтверждение при выходе из программы (кнопка закрытия окна, Alt+F4).</summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!e.Cancel && !App.IsSessionEnding && !ConfirmationHelper.ConfirmExit(this))
            {
                e.Cancel = true;
            }
        }

        private void OnPreviewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == NavigationCommands.BrowseBack && !ConfirmationHelper.ConfirmBack(this))
            {
                e.Handled = true;
            }
        }
    }
}
