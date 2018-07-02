using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using Shopigol.Core.ViewModels;
using Shopigol.WebUI.Models;
using System.Diagnostics;
using System.Linq;

namespace Shopigol.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;

        public HomeController(IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
        }

        public IActionResult Index(string category = null)
        {

            var products = category == null
                ? _productRepository.Collection().ToList() : _productRepository.Collection().Where(p => p.Category == category).ToList();

            var viewModel = new ProductListViewModel
            {
                Products = products,
                ProductCategories = _productCategoryRepository.Collection().ToList()
            };

            return View(viewModel);
        }

        public IActionResult Details(string id)
        {
            var product = _productRepository.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
