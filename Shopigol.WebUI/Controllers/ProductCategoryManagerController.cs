using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Shopigol.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductCategoryManagerController : Controller
    {
        private readonly IRepository<ProductCategory> _productCategoryRepository;

        public ProductCategoryManagerController(IRepository<ProductCategory> productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
        }

        public IActionResult Index()
        {
            var productCategories = _productCategoryRepository.Collection().ToList();

            return View(productCategories);
        }

        public IActionResult Create()
        {
            var productCategory = new ProductCategory();

            return View(productCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }

            _productCategoryRepository.Add(productCategory);
            _productCategoryRepository.Commit();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(string id)
        {
            var productCategoryToEdit = _productCategoryRepository.Find(id);

            if (productCategoryToEdit == null)
            {
                return NotFound();
            }

            return View(productCategoryToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductCategory productCategory)
        {
            var productCategoryToEdit = _productCategoryRepository.Find(productCategory.Id);
            if (productCategoryToEdit == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }

            productCategoryToEdit.Category = productCategory.Category;

            _productCategoryRepository.Commit();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var productCategoryToDelete = _productCategoryRepository.Find(id);

            if (productCategoryToDelete == null)
            {
                return NotFound();
            }

            return View(productCategoryToDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(string id)
        {
            var productCategoryToDelete = _productCategoryRepository.Find(id);

            if (productCategoryToDelete == null)
            {
                return NotFound();
            }

            _productCategoryRepository.Delete(id);
            _productCategoryRepository.Commit();

            return RedirectToAction("Index");
        }
    }
}