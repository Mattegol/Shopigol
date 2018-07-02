using System.Collections.Generic;

namespace Shopigol.Core.Models
{
    public class Basket : BaseEntity
    {
        public virtual ICollection<BasketItem> BasketItems { get; set; }

        public Basket() => BasketItems = new List<BasketItem>();
    }
}
