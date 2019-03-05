using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using Shopigol.Core.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Shopigol.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderContext;

        public OrderService(IRepository<Order> orderContext)
        {
            _orderContext = orderContext;
        }

        public void CreateOrder(Order order, List<BasketItemViewModel> basketItems)
        {
            foreach (var item in basketItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.Id,
                    ProductName = item.ProductName,
                    Image = item.Image,
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }

            _orderContext.Add(order);
            _orderContext.Commit();
        }

        public List<Order> GetOrderList()
        {
            return _orderContext.Collection().ToList();
        }

        public Order GetOrder(string id)
        {
            return _orderContext.Find(id);
        }

        public void UpdateOrder(Order order)
        {
            _orderContext.Update(order);
            _orderContext.Commit();
        }
    }
}
