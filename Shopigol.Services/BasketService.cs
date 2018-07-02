using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;

namespace Shopigol.Services
{
    public class BasketService
    {
        private IRepository<Product> _productRepository;
        private readonly IRepository<Basket> _basketRepository;

        public const string BasketSessionName = "eCommerceBasket";
        private readonly object key;

        public BasketService(IRepository<Product> productRepository, IRepository<Basket> basketRepository)
        {
            _productRepository = productRepository;
            _basketRepository = basketRepository;
        }

        private Basket GetBasket(HttpContext httpContext, bool createIfNull)
        {
            var cookie = httpContext.Request.Cookies[BasketSessionName];

            var basket = new Basket();

            var basketId = cookie;

            if (!string.IsNullOrEmpty(basketId))
            {
                basket = _basketRepository.Find(basketId);
            }
            else
            {
                if (createIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }

            return basket;
        }

        private Basket CreateNewBasket(HttpContext httpContext)
        {
            var basket = new Basket();

            _basketRepository.Add(basket);
            _basketRepository.Commit();

            var option = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            };

            httpContext.Response.Cookies.Append("CookieKey", BasketSessionName, option);

            return basket;
        }

        public void AddToBasket(HttpContext httpContext, string productId)
        {
            var basket = GetBasket(httpContext, createIfNull: true);
            var item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                item = new BasketItem
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };
                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity += 1;
            }

            _basketRepository.Commit();
        }

        public void RemoveFromBasket(HttpContext httpContext, string itemId)
        {
            var basket = GetBasket(httpContext, createIfNull: true);
            var item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (item == null) return;

            basket.BasketItems.Remove(item);
            _basketRepository.Commit();
        }
    }
}
