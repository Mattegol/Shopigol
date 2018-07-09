using Microsoft.AspNetCore.Mvc;
using Moq;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using Shopigol.WebUI.Controllers;
using System.Collections.Generic;
using Xunit;
using static Shopigol.Tests.Fakes.Fakes;

namespace Shopigol.Tests.Controllers
{
    public class ProductCategoryManagerControllerShould
    {

        private readonly Mock<IRepository<ProductCategory>> _mockProductCategoryRepository;
        private ProductCategoryManagerController _sut; //System Under Test

        public ProductCategoryManagerControllerShould()
        {
            _mockProductCategoryRepository = new Mock<IRepository<ProductCategory>>();
            _sut = new ProductCategoryManagerController(_mockProductCategoryRepository.Object);
        }

        [Fact]
        public void ReturnIndexWithProductCategories()
        {
            _mockProductCategoryRepository.Setup(x => x.Collection()).Returns(GetProductCategories());

            IActionResult result = _sut.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<List<ProductCategory>>(viewResult.Model);

            Assert.Equal(2, model.Count);

            Assert.Equal("Sneakers", model[0].Category);
            Assert.Equal("T-Shirt", model[1].Category);
        }

        [Fact]
        public void ReturnCreateWithProductCategory()
        {
            _mockProductCategoryRepository.Setup(x => x.Collection()).Returns(GetProductCategories());

            IActionResult result = _sut.Create();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductCategory>(viewResult.Model);

            Assert.Null(model.Category);
        }

        [Fact]
        public void ReturnCreateWithProductCategoryWhenInvalidModelState()
        {
            _sut.ModelState.AddModelError("x", "Test Error");

            var productCategory = new ProductCategory
            {
                Category = ""
            };

            IActionResult result = _sut.Create(productCategory);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductCategory>(viewResult.Model);

            Assert.Equal(productCategory.Category, model.Category);
        }

        [Fact]
        public void NotSaveProductCategoryWhenModelInvalid()
        {
            _sut.ModelState.AddModelError("x", "Test Error");

            _sut.Create(It.IsAny<ProductCategory>());

            _mockProductCategoryRepository.Verify(
                x => x.Add(It.IsAny<ProductCategory>()), Times.Never());
        }

        [Fact]
        public void SaveProductCategoryWhenValidModel()
        {
            ProductCategory savedProductCategory = null;

            _mockProductCategoryRepository.Setup(x => x.Add(It.IsAny<ProductCategory>()))
                .Callback<ProductCategory>(x => savedProductCategory = x);

            var productCategory = new ProductCategory
            {
                Category = "Sneakers"
            };

            _sut.Create(productCategory);

            _mockProductCategoryRepository.Verify(x => x.Add(productCategory), Times.Once());

            Assert.Equal(productCategory.Category, savedProductCategory.Category);
        }

        [Fact]
        public void RedirectToActionIndexWhenValidModelForCreate()
        {
            var productCategory = new ProductCategory
            {
                Category = "Sneakers"
            };

            var result = _sut.Create(productCategory);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void ReturnNotFoundForEdit()
        {
            IActionResult result = _sut.Edit("abc123");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ReturnEditWithProductCategory()
        {
            _mockProductCategoryRepository.Setup(x => x.Find("abc123")).Returns(GetProductCategory());

            IActionResult result = _sut.Edit("abc123");

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductCategory>(viewResult.Model);

            Assert.Equal(GetProductCategory().Category, model.Category);
        }

        [Fact]
        public void ReturnNotFoundForEditAfterPost()
        {
            IActionResult result = _sut.Edit(new ProductCategory());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ReturnModelStateInvalidForEditAfterPost()
        {
            _sut.ModelState.AddModelError("x", "Test Error");

            _mockProductCategoryRepository.Setup(x => x.Find(GetProductCategory().Id)).Returns(GetProductCategory());

            var productCategory = new ProductCategory
            {
                Id = "abc123",
                Category = ""
            };

            IActionResult result = _sut.Edit(productCategory);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductCategory>(viewResult.Model);

            Assert.Equal(productCategory.Category, model.Category);
        }

        [Fact]
        public void NotEditProductCategoryWhenModelInvalid()
        {
            _sut.ModelState.AddModelError("x", "Test Error");

            _mockProductCategoryRepository.Setup(x => x.Find(GetProductCategory().Id)).Returns(GetProductCategory());

            _sut.Edit(GetProductCategory());

            _mockProductCategoryRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void EditProductCategoryWhenValidModel()
        {
            _mockProductCategoryRepository.Setup(x => x.Find(GetProductCategory().Id)).Returns(GetProductCategory());

            _sut.Edit(GetProductCategory());

            _mockProductCategoryRepository.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public void RedirectToActionIndexWhenValidModelForEdit()
        {
            _mockProductCategoryRepository.Setup(x => x.Find(GetProductCategory().Id)).Returns(GetProductCategory());

            var result = _sut.Edit(GetProductCategory());

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToActionResult.ActionName);
        }


        [Fact]
        public void ReturnNotFoundForDelete()
        {
            IActionResult result = _sut.Delete("abc123");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ReturnDeleteWithProductCategory()
        {
            _mockProductCategoryRepository.Setup(x => x.Find(GetProductCategory().Id)).Returns(GetProductCategory());

            IActionResult result = _sut.Delete(GetProductCategory().Id);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductCategory>(viewResult.Model);

            Assert.Equal(model.Category, GetProductCategory().Category);
        }

        [Fact]
        public void ReturnNotFoundForConfirmDelete()
        {
            IActionResult result = _sut.ConfirmDelete("wrongId");

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void ConfirmDeleteProductCategoryWhenValid()
        {
            _mockProductCategoryRepository.Setup(x => x.Find(GetProductCategory().Id)).Returns(GetProductCategory());

            _sut.ConfirmDelete(GetProductCategory().Id);

            _mockProductCategoryRepository.Verify(
                x => x.Delete(GetProductCategory().Id), Times.Once);

            _mockProductCategoryRepository.Verify(
                x => x.Commit(), Times.Once);
        }

        [Fact]
        public void DeleteProductCategoryWhenValidModel()
        {
            _mockProductCategoryRepository.Setup(x => x.Find(GetProductCategory().Id)).Returns(GetProductCategory());

            _sut.ConfirmDelete(GetProductCategory().Id);

            _mockProductCategoryRepository.Verify(
                x => x.Delete(GetProductCategory().Id), Times.Once);

            _mockProductCategoryRepository.Verify(
                x => x.Commit(), Times.Once);
        }

        [Fact]
        public void RedirectToActionIndexWhenValidForConfirmDelete()
        {
            _mockProductCategoryRepository.Setup(x => x.Find(GetProductCategory().Id)).Returns(GetProductCategory());

            var result = _sut.ConfirmDelete(GetProductCategory().Id);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
