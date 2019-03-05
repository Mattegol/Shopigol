namespace Shopigol.Core.Models
{
    public class OrderItem : BaseEntity
    {
        public string OrderId { get; set; }

        public virtual Order Order { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        public int Quantity { get; set; }

    }
}
