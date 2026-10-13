using System.Windows;

namespace Anastasia423WPF.Windows
{
    public partial class DictionaryEditWindow : Window
    {
        private object _currentItem;

        public DictionaryEditWindow(object item)
        {
            InitializeComponent();
            _currentItem = item;

            // Логика отображения полей в зависимости от типа сущности
            if (_currentItem is Manufacturer m)
            {
                this.Title = "Редактирование производителя";
                CountryStack.Visibility = Visibility.Visible; // Показываем страну

                NameBox.Text = m.Name;
                CountryBox.Text = m.Country; // Заполняем страну из БД
            }
            else if (_currentItem is Anastasia423WPF.Type t)
            {
                this.Title = "Редактирование типа";
                CountryStack.Visibility = Visibility.Collapsed; // Скрываем страну

                NameBox.Text = t.Name;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательного поля Название
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название!");
                return;
            }

            if (_currentItem is Manufacturer m)
            {
                m.Name = NameBox.Text;
                m.Country = CountryBox.Text; // Сохраняем страну

                if (m.ID == 0) Core.Context.Manufacturer.Add(m);
            }
            else if (_currentItem is Anastasia423WPF.Type t)
            {
                t.Name = NameBox.Text;

                if (t.ID == 0) Core.Context.Type.Add(t);
            }

            try
            {
                Core.Context.SaveChanges();
                this.DialogResult = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => this.DialogResult = false;
    }
}