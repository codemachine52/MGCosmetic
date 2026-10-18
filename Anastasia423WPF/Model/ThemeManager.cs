using System;
using System.IO;
using System.Windows;

namespace Anastasia423WPF.Model
{
    /// <summary>Доступные темы оформления.</summary>
    public enum AppTheme
    {
        Light,
        Dark
    }

    /// <summary>
    /// Управляет темой оформления приложения (светлая / тёмная).
    /// Тема — это словарь ресурсов (Themes/LightTheme.xaml или Themes/DarkTheme.xaml),
    /// который подменяется в Application.Resources. Все элементы используют цвета через
    /// DynamicResource, поэтому внешний вид обновляется сразу, без перезапуска.
    /// Выбранная тема сохраняется в файл и восстанавливается при следующем запуске.
    /// </summary>
    public static class ThemeManager
    {
        private const string LightThemeUri = "/Anastasia423WPF;component/Themes/LightTheme.xaml";
        private const string DarkThemeUri = "/Anastasia423WPF;component/Themes/DarkTheme.xaml";

        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MGCosmetic",
            "theme.txt");

        /// <summary>Текущая тема. По умолчанию в App.xaml подключена светлая.</summary>
        public static AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

        /// <summary>Возникает после смены темы.</summary>
        public static event EventHandler ThemeChanged;

        /// <summary>Применяет сохранённую тему. Вызывается один раз при запуске приложения.</summary>
        public static void Initialize()
        {
            AppTheme saved = LoadSavedTheme();
            if (saved != CurrentTheme)
            {
                Apply(saved, false);
            }
        }

        /// <summary>Переключает тему на противоположную и запоминает выбор.</summary>
        public static void Toggle()
        {
            Apply(CurrentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light, true);
        }

        private static void Apply(AppTheme theme, bool save)
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var newDictionary = new ResourceDictionary
            {
                Source = new Uri(theme == AppTheme.Dark ? DarkThemeUri : LightThemeUri, UriKind.Relative)
            };

            int index = FindThemeDictionaryIndex(dictionaries);
            if (index >= 0)
            {
                dictionaries[index] = newDictionary;
            }
            else
            {
                dictionaries.Insert(0, newDictionary);
            }

            CurrentTheme = theme;

            if (save)
            {
                SaveTheme(theme);
            }

            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        private static int FindThemeDictionaryIndex(System.Collections.Generic.IList<ResourceDictionary> dictionaries)
        {
            for (int i = 0; i < dictionaries.Count; i++)
            {
                Uri source = dictionaries[i].Source;
                if (source != null && source.OriginalString.IndexOf("Theme.xaml", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private static AppTheme LoadSavedTheme()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string text = File.ReadAllText(SettingsFilePath).Trim();
                    if (text.Equals(AppTheme.Dark.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return AppTheme.Dark;
                    }
                }
            }
            catch (Exception)
            {
                // Файл настроек недоступен или повреждён — используем тему по умолчанию.
            }
            return AppTheme.Light;
        }

        private static void SaveTheme(AppTheme theme)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));
                File.WriteAllText(SettingsFilePath, theme.ToString());
            }
            catch (Exception)
            {
                // Не удалось сохранить выбор темы (нет прав и т.п.) — это не критично,
                // тема всё равно применена на время работы приложения.
            }
        }
    }
}
