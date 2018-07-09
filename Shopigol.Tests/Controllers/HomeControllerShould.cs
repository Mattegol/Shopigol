using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using Shopigol.Core.ViewModels;
using Shopigol.WebUI.Controllers;
using Xunit;
using static Shopigol.Tests.Fakes.Fakes;

namespace Shopigol.Tests.Controllers
{
    public class HomeControllerShould
    {
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly Mock<IRepository<ProductCategory>> _mockProductCategoryRepository;
        private readonly HomeController _sut;

        public HomeControllerShould()
        {
            _mockProductRepository = new Mock<IRepository<Product>>();
            _mockProductCategoryRepository = new Mock<IRepository<ProductCategory>>();
            _sut = new HomeController(_mockProductRepository.Object, _mockProductCategoryRepository.Object);
        }

        [Fact]
        public void ReturnIndexWithProductListViewModel()
        {
            _mockProductCategoryRepository.Setup(x => x.Collection()).Returns(GetProductCategories());

            IActionResult result = _sut.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductListViewModel>(viewResult.Model);

            Assert.Empty(model.Products);

            Assert.Equal(2, model.ProductCategories.Count());
        }

        [Fact]
        public void ReturnIndexWithProductListViewModelIncludingProducts()
        {
            _mockProductRepository.Setup(x => x.Collection()).Returns(GetProducts());

            _mockProductCategoryRepository.Setup(x => x.Collection()).Returns(GetProductCategories());

            IActionResult result = _sut.Index("Sneakers");

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ProductListViewModel>(viewResult.Model);

            Assert.Equal(2, model.Products.Count());

            Assert.Equal(2, model.ProductCategories.Count());
        }

        [Fact]
        public void ReturnNotFoundDetails()
        {
            _mockProductRepository.Setup(x => x.Find(GetProduct().Id)).Returns(GetProduct());

            IActionResult result = _sut.Details("wrongId");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ReturnDetailsWithProduct()
        {
            _mockProductRepository.Setup(x => x.Find(GetProduct().Id)).Returns(GetProduct());

            IActionResult result = _sut.Details(GetProduct().Id);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<Product>(viewResult.Model);

            Assert.Equal(GetProduct().Category, model.Category);
            Assert.Equal(GetProduct().Name, model.Name);
            Assert.Equal(GetProduct().Image, model.Image);
            Assert.Equal(GetProduct().Description, model.Description);
            Assert.Equal(GetProduct().Price, model.Price);

        }
    }
}
