using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anastasia423WPF
{
    public partial class Product
    {
        public bool HasBigDiscount
        {
            get
            {
                return Discount > 15;
            }
        }
        public int CountProd { get; set; } = 1;
        public decimal PriceWithDiscount => Price * (1 - (decimal)Discount / 100);
    }
}
