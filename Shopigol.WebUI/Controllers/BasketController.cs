using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using System.Linq;

namespace Shopigol.WebUI.Controllers
{
    public class BasketController : Controller
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public BasketController(IBasketService basketService,
            IOrderService orderService,
            IRepository<Customer> customerRepository)
        {
            _basketService = basketService;
            _orderService = orderService;
            _customerRepository = customerRepository;
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

        [Authorize]
        public IActionResult Checkout()
        {
            var customer = _customerRepository.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

            if (customer != null)
            {
                var order = new Order
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Street = customer.Street,
                    City = customer.City,
                    State = customer.State,
                    ZipCode = customer.ZipCode
                };

                return View(order);
            }

            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Checkout(Order order)
        {
            var basketItems = _basketService.GetBasketItems(HttpContext);

            order.Email = User.Identity.Name;

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