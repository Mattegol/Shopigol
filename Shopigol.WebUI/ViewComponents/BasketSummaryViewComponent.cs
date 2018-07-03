using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Contracts;

namespace Shopigol.WebUI.ViewComponents
{
    public class BasketSummaryViewComponent : ViewComponent
    {
        private readonly IBasketService _basketService;

        public BasketSummaryViewComponent(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public IViewComponentResult Invoke()
        {
            var basketSummary = _basketService.GetBasketSummary(HttpContext);

            return View("Default", basketSummary);
        }
    }
}
