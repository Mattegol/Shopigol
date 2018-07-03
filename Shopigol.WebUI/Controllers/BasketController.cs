using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Contracts;

namespace Shopigol.WebUI.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
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

    }
}