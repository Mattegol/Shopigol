﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using Shopigol.Core.ViewModels;
using Shopigol.WebUI.Controllers;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace Shopigol.Tests.Controllers
{
    public class BasketControllerShould
    {
        private readonly Mock<IBasketService> _basketService;
        private readonly Mock<IOrderService> _orderService;
        private readonly Mock<IPrincipal> _mockPrincipal;
        private readonly Mock<IRepository<Customer>> _customerRepository;
        private readonly Mock<IRepository<Product>> _productRepository;
        private readonly Mock<IRepository<Basket>> _basketRepository;
        private readonly Mock<IRepository<Order>> _orderRepository;
        private readonly BasketController _sut; //System Under Test

        public BasketControllerShould()
        {
            _mockPrincipal = new Mock<IPrincipal>();
            _customerRepository = new Mock<IRepository<Customer>>();
            _productRepository = new Mock<IRepository<Product>>();
            _basketRepository = new Mock<IRepository<Basket>>();
            _orderRepository = new Mock<IRepository<Order>>();
            _basketService = new Mock<IBasketService>();
            _orderService = new Mock<IOrderService>();
            _sut = new BasketController(_basketService.Object, _orderService.Object, _customerRepository.Object);
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

        [Fact]
        public void CheckoutAndCreateOrder()
        {
            // Arrange

            var basketItems = new List<BasketItemViewModel>
            {
                new BasketItemViewModel
                {
                    Id = "1",
                    Quantity = 2,
                    ProductName = "P1",
                    Price = 10.00m
                },
                new BasketItemViewModel
                {
                    Id = "2",
                    Quantity = 1,
                    ProductName = "P2",
                    Price = 5.00m
                }
            };

            _basketService.Setup(x => x.GetBasketItems(It.IsAny<HttpContext>())).Returns(basketItems);

            var order = new Order();
            _orderService.Setup(x => x.CreateOrder(order, basketItems));

            var customer = new Customer
            {
                Id = "1",
                Email = "john.doe@shopigol.com",
                City = "Stockholm"
            };

            _customerRepository.Setup(x => x.Add(customer));

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, customer.Email),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("name", customer.Email),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);


            _mockPrincipal.Setup(x => x.Identity).Returns(identity);
            _mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);

            var context = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Then set it to controller before executing test
            _sut.ControllerContext = context;

            // Act

            var result = _sut.Checkout(order);

            // Assert

            Assert.Equal("Payment Processed", order.OrderStatus);

            _basketService.Verify(x => x.GetBasketItems(It.IsAny<HttpContext>()), Times.Once);

            Assert.Equal(2, _basketService.Object.GetBasketItems(It.IsAny<HttpContext>()).Count);

            _orderService.Verify(x => x.CreateOrder(order, basketItems), Times.Once);

            _basketService.Verify(x => x.ClearBasket(It.IsAny<HttpContext>()), Times.Once);

            // Act

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("ThankYou", redirectToActionResult.ActionName);


            // Assert


        }
    }
}
