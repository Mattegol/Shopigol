using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shopigol.Core.Contracts;
using Shopigol.Core.ViewModels;
using Shopigol.WebUI.Controllers;
using Xunit;

namespace Shopigol.Tests.Controllers
{
    public class BasketControllerShould
    {
        private readonly Mock<IBasketService> _basketService;
        private readonly BasketController _sut; //System Under Test

        public BasketControllerShould()
        {
            _basketService = new Mock<IBasketService>();
            _sut = new BasketController(_basketService.Object);
        }

        [Fact]
        public void ReturnIndexWithBasketItemViewModel()
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();

            _basketService.Setup(x => x.GetBasketItems(It.IsAny<HttpContext>())).Returns(basketItems);

            IActionResult result = _sut.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<List<BasketItemViewModel>>(viewResult.Model);
        }

        [Fact]
        public void RedirectToActionIndexWhenValidForAddToBasket()
        {
            _basketService.Setup(x => x.AddToBasket(It.IsAny<HttpContext>(), "abc123"));

            var result = _sut.AddToBasket("abc123");

            _basketService.Verify(x => x.AddToBasket(It.IsAny<HttpContext>(), "abc123"), Times.Once);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToActionResult.ActionName);
        }


        [Fact]
        public void RedirectToActionIndexWhenValidForRemoveFromBasket()
        {
            _basketService.Setup(x => x.RemoveFromBasket(It.IsAny<HttpContext>(), "abc123"));

            var result = _sut.RemoveFromBasket("abc123");

            _basketService.Verify(x => x.RemoveFromBasket(It.IsAny<HttpContext>(), "abc123"), Times.Once);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
