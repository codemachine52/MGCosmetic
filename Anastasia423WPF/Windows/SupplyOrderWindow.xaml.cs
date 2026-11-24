using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Anastasia423WPF.Windows
{
    public partial class SupplyOrderWindow : Window
    {
        private class OrderLine
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
            public string Name => Product.Name;
        }

        private List<OrderLine> _lines = new List<OrderLine>();

        public SupplyOrderWindow()
        {
            InitializeComponent();
            SupplierBox.ItemsSource = Core.Context.Supplier.ToList();
            ProductBox.ItemsSource = Core.Context.Product.ToList();
        }

        private void RefreshLines()
        {
            LinesGrid.ItemsSource = null;
            LinesGrid.ItemsSource = _lines;
        }

        private void AddLine_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProductBox.SelectedItem is Product product))
            {
                MessageBox.Show("Выберите товар!");
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите количество - целое число больше нуля!");
                return;
            }

            OrderLine line = _lines.FirstOrDefault(l => l.Product.ID == product.ID);
            if (line == null)
            {
                _lines.Add(new OrderLine { Product = product, Quantity = quantity });
            }
            else
            {
                line.Quantity += quantity;
            }

            RefreshLines();
            QuantityBox.Clear();
        }

        private void RemoveLine_Click(object sender, RoutedEventArgs e)
        {
            if (LinesGrid.SelectedItem is OrderLine selected)
            {
                _lines.Remove(selected);
                RefreshLines();
            }
            else
            {
                MessageBox.Show("Выберите товар в заказе!");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!(SupplierBox.SelectedItem is Supplier supplier))
            {
                MessageBox.Show("Выберите поставщика!");
                return;
            }

            if (_lines.Count == 0)
            {
                MessageBox.Show("Добавьте в заказ хотя бы один товар!");
                return;
            }

            try
            {
                SupplyOrder order = new SupplyOrder
                {
                    SupplierID = supplier.ID,
                    OrderDate = DateTime.Now,
                    Status = "Оформлен"
                };

                foreach (var line in _lines)
                {
                    order.SupplyOrder_Product.Add(new SupplyOrder_Product
                    {
                        ProductID = line.Product.ID,
                        Quantity = line.Quantity
                    });
                }

                Core.Context.SupplyOrder.Add(order);
                Core.Context.SaveChanges();

                MessageBox.Show($"Заказ поставщику №{order.ID} оформлен!");
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
