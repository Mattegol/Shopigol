namespace Shopigol.Core.Models
{
    public class BasketItem : BaseEntity
    {
        public string BasketId { get; set; }

        public virtual Basket Basket { get; set; }

        public string ProductId { get; set; }

        public int Quantity { get; set; }

    }
}