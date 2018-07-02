using Microsoft.AspNetCore.Http;
using Shopigol.Core.ViewModels;
using System.Collections.Generic;

namespace Shopigol.Core.Contracts
{
    public interface IBasketService
    {
        void AddToBasket(HttpContext httpContext, string productId);

        void RemoveFromBasket(HttpContext httpContext, string itemId);

        List<BasketItemViewModel> GetBasketItems(HttpContext httpContext);

        BasketSummaryViewModel GetBasketSummary(HttpContext httpContext);
    }
}
