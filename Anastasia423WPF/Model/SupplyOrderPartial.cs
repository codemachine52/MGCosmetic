using System.Linq;

namespace Anastasia423WPF
{
    public partial class SupplyOrder
    {
        public string Composition => string.Join(", ", SupplyOrder_Product.Select(p => p.Product.Name + " × " + p.Quantity));
    }
}
