using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using Shopigol.Core.ViewModels;
using System.IO;
using System.Linq;

namespace Shopigol.WebUI.Controllers
{
    public class ProductManagerController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IHostingEnvironment _environment;

        public ProductManagerController(IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IHostingEnvironment environment)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _environment = environment;
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
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new ProductManagerViewModel
                {
                    Product = product,
                    ProductCategories = _productCategoryRepository.Collection()
                };
                return View(viewModel);
            }

            if (file != null)
            {
                product.Image = product.Id + Path.GetExtension(file.FileName);

                var uploadsDirectoryPath = Path.Combine(_environment.WebRootPath, "images");
                var uploadedfilePath = Path.Combine(uploadsDirectoryPath, product.Image);

                using (var stream = new FileStream(uploadedfilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
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
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product, IFormFile file)
        {
            var productToEdit = _productRepository.Find(product.Id);
            if (productToEdit == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var viewModel = new ProductManagerViewModel
                {
                    Product = productToEdit,
                    ProductCategories = _productCategoryRepository.Collection()
                };
                return View(viewModel);
            }

            if (file != null)
            {
                productToEdit.Image = product.Id + Path.GetExtension(file.FileName);

                var uploadsDirectoryPath = Path.Combine(_environment.WebRootPath, "images");
                var uploadedfilePath = Path.Combine(uploadsDirectoryPath, productToEdit.Image);

                using (var stream = new FileStream(uploadedfilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            productToEdit.Name = product.Name;
            productToEdit.Category = product.Category;
            productToEdit.Description = product.Description;
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
        [ValidateAntiForgeryToken]
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