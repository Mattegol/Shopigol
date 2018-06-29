using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Models;
using Shopigol.Core.ViewModels;
using Shopigol.DataAccess.InMemory;
using System.Linq;

namespace Shopigol.WebUI.Controllers
{
    public class ProductManagerController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly ProductCategoryRepository _productCategoryRepository;


        public ProductManagerController()
        {
            _productRepository = new ProductRepository();
            _productCategoryRepository = new ProductCategoryRepository();
        }


        public IActionResult Index()
        {
            var products = _productRepository.Collection().ToList();

            return View(products);
        }

        public IActionResult Create()
        {
            var viewModel = new ProductManagerViewModel
            {
                Product = new Product(),
                ProductCategories = _productCategoryRepository.Collection()
            };


            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            _productRepository.Add(product);
            _productRepository.Commit();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(string id)
        {
            var productToEdit = _productRepository.Find(id);

            if (productToEdit == null)
            {
                return NotFound();
            }

            var viewModel = new ProductManagerViewModel
            {
                Product = productToEdit,
                ProductCategories = _productCategoryRepository.Collection()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            var productToEdit = _productRepository.Find(product.Id);
            if (productToEdit == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            productToEdit.Name = product.Name;
            productToEdit.Category = product.Category;
            productToEdit.Description = product.Description;
            productToEdit.Image = product.Image;
            productToEdit.Price = product.Price;

            _productRepository.Commit();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var productToDelete = _productRepository.Find(id);

            if (productToDelete == null)
            {
                return NotFound();
            }

            return View(productToDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult ConfirmDelete(string id)
        {
            var productToDelete = _productRepository.Find(id);

            if (productToDelete == null)
            {
                return NotFound();
            }

            _productRepository.Delete(id);
            _productRepository.Commit();

            return RedirectToAction("Index");
        }
    }
}