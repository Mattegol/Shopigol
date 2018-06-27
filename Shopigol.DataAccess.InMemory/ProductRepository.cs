using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Shopigol.Core.Models;

namespace Shopigol.DataAccess.InMemory
{
    public class ProductRepository
    {
        readonly ObjectCache _cache = MemoryCache.Default;
        private readonly List<Product> _products;

        public ProductRepository()
        {
            _products = _cache["products"] as List<Product> ?? new List<Product>();
        }

        public void Commit()
        {
            _cache["products"] = _products;
        }

        public void Add(Product product)
        {
            _products.Add(product);
        }

        public void Update(Product product)
        {
            var productToUpdate = _products.Find(p => p.Id == product.Id);

            if (productToUpdate != null)
            {
                productToUpdate = product;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public Product Find(string id)
        {
            var product = _products.Find(p => p.Id == id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }
            return product;
        }

        public IQueryable<Product> Collection()
        {
            return _products.AsQueryable();
        }

        public void Delete(string id)
        {
            var productToDelete = _products.Find(p => p.Id == id);

            if (productToDelete != null)
            {
                _products.Remove(productToDelete);
            }
            else
            {
                throw new Exception("Product not found");
            }
        }
    }
}
