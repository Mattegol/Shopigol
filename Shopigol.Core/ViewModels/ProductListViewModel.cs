using Shopigol.Core.Models;
using System.Collections.Generic;

namespace Shopigol.Core.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<ProductCategory> ProductCategories { get; set; }



    }
}
