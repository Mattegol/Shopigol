using System;

namespace Shopigol.Core.Models
{
    public class ProductCategory
    {
        public string Id { get; set; }

        public string Category { get; set; }

        public ProductCategory()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
