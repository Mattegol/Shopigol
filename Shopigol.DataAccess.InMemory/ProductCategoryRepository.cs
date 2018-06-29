using Shopigol.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Shopigol.DataAccess.InMemory
{
    public class ProductCategoryRepository
    {
        readonly ObjectCache _cache = MemoryCache.Default;
        private readonly List<ProductCategory> _productCategories;

        public ProductCategoryRepository()
        {
            _productCategories = _cache["productCategories"] as List<ProductCategory> ?? new List<ProductCategory>();
        }

        public void Commit()
        {
            _cache["productCategories"] = _productCategories;
        }

        public void Add(ProductCategory productCategory)
        {
            _productCategories.Add(productCategory);
        }

        public void Update(ProductCategory productCategory)
        {
            var productCategoryToUpdate = _productCategories.Find(p => p.Id == productCategory.Id);

            if (productCategoryToUpdate != null)
            {
                productCategoryToUpdate = productCategory;
            }
            else
            {
                throw new Exception("Product category not found");
            }
        }

        public ProductCategory Find(string id)
        {
            var productCategory = _productCategories.Find(p => p.Id == id);

            if (productCategory == null)
            {
                throw new Exception("Product Category not found");
            }
            return productCategory;
        }

        public IQueryable<ProductCategory> Collection()
        {
            return _productCategories.AsQueryable();
        }

        public void Delete(string id)
        {
            var productCategoryToDelete = _productCategories.Find(p => p.Id == id);

            if (productCategoryToDelete != null)
            {
                _productCategories.Remove(productCategoryToDelete);
            }
            else
            {
                throw new Exception("Product Category not found");
            }
        }
    }

}
