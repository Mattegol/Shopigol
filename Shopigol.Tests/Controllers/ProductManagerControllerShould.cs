using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using Shopigol.Core.ViewModels;
using Shopigol.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Shopigol.Tests.Fakes.Fakes;
using ProductCategory = Shopigol.Core.Models.ProductCategory;

namespace Shopigol.Tests.Controllers
{
    public class ProductManagerControllerShould
    {
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly Mock<IRepository<ProductCategory>> _mockProductCategoryRepository;
        private readonly Mock<IHostingEnvironment> _mockEnvironment;
        private ProductManagerController _sut; //System Under Test

        public ProductManagerControllerShould()
        {
            _mockProductRepository = new Mock<IRepository<Product>>();
            _mockProductCategoryRepository = new Mock<IRepository<ProductCategory>>();
            _mockEnvironment = new Mock<IHostingEnvironment>();

            //_sut = new ProductManagerController(_mock);
        }


        [Fact]
        public void ReturnIndexWithProducts()
        {
            _mockProductRepository.Setup(products => products.Collection()).Returns(GetProducts());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            IActionResult result = _sut.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<List<Product>>(viewResult.Model);

            Assert.Equal(2, model.Count);

            Assert.Equal("White sneakers", model[0].Description);
            Assert.Equal("Black sneakers", model[1].Description);
        }

        [Fact]
        public void ReturnCreateWithProductManagerViewModel()
        {
            _mockProductCategoryRepository.Setup(productCategories => productCategories.Collection())
                .Returns(GetProductCategories());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            IActionResult result = _sut.Create();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductManagerViewModel>(viewResult.Model);

            Assert.Equal(2, model.ProductCategories.Count());
            Assert.Equal("Sneakers", model.ProductCategories.First().Category);
        }

        [Fact]
        public void ReturnCreateWithProductManagerViewModelWhenInvalidModelState()
        {
            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            _sut.ModelState.AddModelError("x", "Test Error");

            var product = new Product
            {
                Category = "Sneakers"
            };

            IFormFile file = null;

            IActionResult result = _sut.Create(product, file);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductManagerViewModel>(viewResult.Model);

            Assert.Equal(product.Category, model.Product.Category);
        }

        [Fact]
        public void NotSaveProductWhenModelError()
        {
            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            _sut.ModelState.AddModelError("x", "Test Error");

            var product = new Product
            {
                Category = "Sneakers"
            };

            IFormFile file = null;

            _sut.Create(product, file);

            _mockProductRepository.Verify(
                x => x.Add(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public void SaveProductWhenValidModel()
        {
            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            Product savedProduct = null;

            _mockProductRepository.Setup(x => x.Add(It.IsAny<Product>()))
                .Callback<Product>(x => savedProduct = x);

            var product = new Product
            {
                Category = "Sneakers",
                CreatedAt = DateTimeOffset.Now,
                Description = "White sneakers",
                Id = "abc123",
                Image = "/path/image.jpg",
                Name = "Nike Air Force One",
                Price = 10
            };

            IFormFile file = null;

            _sut.Create(product, file);

            _mockProductRepository.Verify(
                x => x.Add(It.IsAny<Product>()), Times.Once);

            Assert.Equal(product.Category, savedProduct.Category);
            Assert.Equal(product.CreatedAt, savedProduct.CreatedAt);
            Assert.Equal(product.Description, savedProduct.Description);
            Assert.Equal(product.Id, savedProduct.Id);
            Assert.Equal(product.Image, savedProduct.Image);
            Assert.Equal(product.Name, savedProduct.Name);
            Assert.Equal(product.Price, savedProduct.Price);
        }

        [Fact]
        public void RedirectToActionIndexWhenValidModelForCreate()
        {
            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            var product = new Product
            {
                Category = "Sneakers",
                CreatedAt = DateTimeOffset.Now,
                Description = "White sneakers",
                Id = "abc123",
                Image = "/path/image.jpg",
                Name = "Nike Air Force One",
                Price = 10
            };

            IFormFile file = null;

            var result = _sut.Create(product, file);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void ReturnNotFoundForEdit()
        {
            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            IActionResult result = _sut.Edit("abc123");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ReturnEditWithProductManagerViewModel()
        {
            _mockProductRepository.Setup(products => products.Find("abc123")).Returns(GetProduct());

            _mockProductCategoryRepository.Setup(productCategories => productCategories.Collection()).Returns(GetProductCategories());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            IActionResult result = _sut.Edit("abc123");

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductManagerViewModel>(viewResult.Model);

            Assert.Equal(GetProduct().Category, model.Product.Category);
            Assert.Equal(GetProduct().Id, model.Product.Id);
            Assert.Equal(GetProduct().CreatedAt.Minute, model.Product.CreatedAt.Minute);
            Assert.Equal(GetProduct().Description, model.Product.Description);
            Assert.Equal(GetProduct().Name, model.Product.Name);
            Assert.Equal(GetProduct().Price, model.Product.Price);
            Assert.Equal(GetProduct().Image, model.Product.Image);

            Assert.Equal(GetProductCategories().First().Category, model.ProductCategories.First().Category);
        }

        [Fact]
        public void ReturnNotFoundForEditAfterPost()
        {
            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object, _mockEnvironment.Object);

            IFormFile file = null;

            IActionResult result = _sut.Edit(new Product(), file);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ReturnEditWithProductManagerViewModelWhenInvalidModelState()
        {
            _mockProductRepository.Setup(products => products.Find("abc123")).Returns(GetProduct());

            _mockProductCategoryRepository.Setup(productCategories => productCategories.Collection()).Returns(GetProductCategories());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object, _mockEnvironment.Object);

            _sut.ModelState.AddModelError("x", "Test Error");

            var product = new Product
            {
                Category = "Sneakers",
                Id = "abc123"
            };

            IFormFile file = null;

            IActionResult result = _sut.Edit(product, file);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductManagerViewModel>(viewResult.Model);

            Assert.Equal(product.Category, model.Product.Category);
        }

        [Fact]
        public void NotEditProductWhenModelError()
        {
            _mockProductRepository.Setup(products => products.Find("abc123")).Returns(GetProduct());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            _sut.ModelState.AddModelError("x", "Test Error");

            var product = new Product
            {
                Category = "Sneakers",
                Id = "abc123"
            };

            IFormFile file = null;

            _sut.Edit(product, file);

            _mockProductRepository.Verify(
                x => x.Update(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public void EditProductWhenValidModel()
        {
            _mockProductRepository.Setup(products => products.Find("abc123")).Returns(GetProduct());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            var product = new Product
            {
                Category = "Sneakers",
                CreatedAt = DateTimeOffset.Now,
                Description = "White sneakers",
                Id = "abc123",
                Image = "/path/image.jpg",
                Name = "Nike Air Force One",
                Price = 10
            };

            IFormFile file = null;

            _sut.Edit(product, file);

            _mockProductRepository.Verify(
                x => x.Commit(), Times.Once);
        }

        [Fact]
        public void RedirectToActionIndexWhenValidModelForEdit()
        {
            _mockProductRepository.Setup(products => products.Find("abc123")).Returns(GetProduct());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            var product = new Product
            {
                Category = "Sneakers",
                CreatedAt = DateTimeOffset.Now,
                Description = "White sneakers",
                Id = "abc123",
                Image = "/path/image.jpg",
                Name = "Nike Air Force One",
                Price = 10
            };

            IFormFile file = null;

            var result = _sut.Edit(product, file);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void ReturnNotFoundForDelete()
        {
            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            IActionResult result = _sut.Delete("abc123");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ReturnDeleteWithProduct()
        {
            _mockProductRepository.Setup(products => products.Find("abc123")).Returns(GetProduct());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            IActionResult result = _sut.Delete("abc123");

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<Product>(viewResult.Model);

            Assert.Equal((string)GetProduct().Category, model.Category);
            Assert.Equal((string)GetProduct().Id, model.Id);
            Assert.Equal(GetProduct().CreatedAt.Minute, model.CreatedAt.Minute);
            Assert.Equal((string)GetProduct().Description, model.Description);
            Assert.Equal((string)GetProduct().Name, model.Name);
            Assert.Equal(GetProduct().Price, model.Price);
            Assert.Equal((string)GetProduct().Image, model.Image);
        }


        [Fact]
        public void ReturnNotFoundForConfirmDelete()
        {
            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            IActionResult result = _sut.ConfirmDelete("abc123");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ConfirmDeleteProductWhenValid()
        {
            _mockProductRepository.Setup(products => products.Find("abc123")).Returns(GetProduct());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            _sut.ConfirmDelete("abc123");

            _mockProductRepository.Verify(
                x => x.Delete("abc123"), Times.Once);

            _mockProductRepository.Verify(
                x => x.Commit(), Times.Once);
        }

        [Fact]
        public void RedirectToActionIndexWhenValidForConfirmDelete()
        {
            _mockProductRepository.Setup(products => products.Find("abc123")).Returns(GetProduct());

            _sut = new ProductManagerController(_mockProductRepository.Object, _mockProductCategoryRepository.Object,
                _mockEnvironment.Object);

            var result = _sut.ConfirmDelete("abc123");

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
