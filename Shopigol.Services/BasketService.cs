using Microsoft.AspNetCore.Http;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using Shopigol.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shopigol.Services
{
    public class BasketService : IBasketService
    {
        private readonly IRepository<Product> _productRepository;
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

        public List<BasketItemViewModel> GetBasketItems(HttpContext httpContext)
        {
            var basket = GetBasket(httpContext, createIfNull: false);

            if (basket == null) return new List<BasketItemViewModel>();

            return (from b in basket.BasketItems
                    join p in _productRepository.Collection() on b.ProductId equals p.Id
                    select new BasketItemViewModel
                    {
                        Id = b.Id,
                        Quantity = b.Quantity,
                        ProductName = p.Name,
                        Image = p.Image,
                        Price = p.Price
                    }).ToList();
        }

        public BasketSummaryViewModel GetBasketSummary(HttpContext httpContext)
        {
            var basket = GetBasket(httpContext, createIfNull: false);

            var viewModel = new BasketSummaryViewModel(0, 0);

            if (basket == null) return viewModel;

            int? basketCount = (from item in basket.BasketItems
                select item.Quantity).Sum();

            decimal? basketTotal = (from item in basket.BasketItems
                join p in _productRepository.Collection() on item.ProductId equals p.Id
                select item.Quantity * p.Price).Sum();

            viewModel.BasketCount = (int) basketCount;
            viewModel.BasketTotal = (decimal) basketTotal;

            return viewModel;
        }
    }
}
