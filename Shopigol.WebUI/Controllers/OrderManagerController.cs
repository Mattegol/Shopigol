using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;

namespace Shopigol.WebUI.Controllers
{
    public class OrderManagerController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderManagerController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            var orderList = _orderService.GetOrderList();

            return View(orderList);
        }

        public IActionResult UpdateOrder(string id)
        {
            var order = _orderService.GetOrder(id);

            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateOrder(Order order, string id)
        {
            var orderToBeUpdated = _orderService.GetOrder(id);

            orderToBeUpdated.OrderStatus = order.OrderStatus;

            _orderService.UpdateOrder(orderToBeUpdated);

            return RedirectToAction("Index");
        }

    }
}