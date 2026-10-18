using System;
using Anastasia423WPF.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Anastasia423WPF
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// true, когда Windows завершает сеанс (выключение / выход из системы).
        /// В этом случае окно подтверждения выхода из программы не показываем.
        /// </summary>
        public static bool IsSessionEnding { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Применяем сохранённую тему до создания главного окна
            ThemeManager.Initialize();
            base.OnStartup(e);
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            IsSessionEnding = true;
            base.OnSessionEnding(e);
            if (e.Cancel)
            {
                IsSessionEnding = false;
            }
        }
    }
}
