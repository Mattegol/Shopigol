using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shopigol.Core.Models;
using Shopigol.DataAccess.InMemory;

namespace Shopigol.WebUI.Controllers
{
    public class ProductCategoryManagerController : Controller
    {
        private readonly InMemoryRepository<ProductCategory> _productCategoryRepository;

        public ProductCategoryManagerController()
        {
            _productCategoryRepository = new InMemoryRepository<ProductCategory>();
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