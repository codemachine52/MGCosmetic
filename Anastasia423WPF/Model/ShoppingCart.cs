using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anastasia423WPF.Model
{
    public static class ShoppingCart
    {
        public static List<Product> Items { get; set; } = new List<Product>();

        // false, если в корзине уже всё, что есть на складе
        public static bool Add(Product product)
        {
            bool inCart = Items.Contains(product);
            int count = inCart ? product.CountProd : 0;

            if (count >= product.Quantity)
            {
                return false;
            }

            if (inCart)
            {
                product.CountProd++;
            }
            else
            {
                Items.Add(product);
            }
            return true;
        }
    }

    public partial class Order
    {
        // Свойство для отображения общей суммы в корзине
        public decimal TotalCost
        {
            get
            {
                // Считаем сумму всех товаров с учетом их скидок
                return (decimal)ShoppingCart.Items.Sum(p => (double)p.Price * (1 - p.Discount / 100));
            }
        }
    }
}
