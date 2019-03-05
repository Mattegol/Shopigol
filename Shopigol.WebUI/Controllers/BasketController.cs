using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;

namespace Shopigol.WebUI.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public BasketController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            var viewModel = _basketService.GetBasketItems(HttpContext);

            return View(viewModel);
        }

        public IActionResult AddToBasket(string id)
        {
            _basketService.AddToBasket(HttpContext, id);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromBasket(string id)
        {
            _basketService.RemoveFromBasket(HttpContext, id);

            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            var basketItems = _basketService.GetBasketItems(HttpContext);
            //order.OrderStatus = "Order Created";

            // Process payment

            order.OrderStatus = "Payment Processed";

            _orderService.CreateOrder(order, basketItems);
            _basketService.ClearBasket(HttpContext);

            return RedirectToAction("ThankYou", new { OrderId = order.Id });
        }

        public IActionResult ThankYou(string orderId)
        {
            ViewBag.OrderId = orderId;

            return View();
        }
    }
}