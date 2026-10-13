using System;
using System.Linq;
using System.Windows;

namespace Anastasia423WPF.Windows
{
    public partial class AddEditProductWindow : Window
    {
        private Product _currentProduct = new Product();
        private bool _isEdit = false;

        // Конструктор для окна (принимает null для нового, или конкретный товар для редактирования)
        public AddEditProductWindow(Product selectedProduct)
        {
            InitializeComponent();

            // Загружаем списки для ComboBox
            ComboManufacturer.ItemsSource = Core.Context.Manufacturer.ToList();
            ComboType.ItemsSource = Core.Context.Type.ToList();

            if (selectedProduct != null)
            {
                _currentProduct = selectedProduct;
                _isEdit = true;

                // Заполняем поля
                TxtName.Text = _currentProduct.Name;
                TxtPrice.Text = _currentProduct.Price.ToString();
                TxtDiscount.Text = _currentProduct.Discount.ToString();
                ComboManufacturer.SelectedValue = _currentProduct.ManufacturerID;
                ComboType.SelectedValue = _currentProduct.TypeID;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentProduct.Name = TxtName.Text;
                _currentProduct.Price = Convert.ToDecimal(TxtPrice.Text);
                _currentProduct.Discount = string.IsNullOrWhiteSpace(TxtDiscount.Text) ? 0 : Convert.ToByte(TxtDiscount.Text);
                _currentProduct.ManufacturerID = (int)ComboManufacturer.SelectedValue;
                _currentProduct.TypeID = (int)ComboType.SelectedValue;

                if (!_isEdit)
                {
                    Core.Context.Product.Add(_currentProduct);
                }

                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: проверьте правильность ввода данных. " + ex.Message);
            }
        }
    }
}