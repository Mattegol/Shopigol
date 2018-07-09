using Shopigol.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shopigol.Tests.Fakes
{
    public static class Fakes
    {
        public static Product GetProduct()
        {
            return new Product
            {
                Category = "Sneakers",
                CreatedAt = DateTimeOffset.Now,
                Description = "White sneakers",
                Id = "abc123",
                Image = "/path/image.jpg",
                Name = "Nike Air Force One",
                Price = 10
            };
        }

        public static IQueryable<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Category = "Sneakers",
                    CreatedAt = DateTimeOffset.Now,
                    Description = "White sneakers",
                    Id = "abc123",
                    Image = "/path/image.jpg",
                    Name = "Nike Air Force One",
                    Price = 10
                },
                new Product
                {
                    Category = "Sneakers",
                    CreatedAt = DateTimeOffset.Now,
                    Description = "Black sneakers",
                    Id = "def456",
                    Image = "/path/image.jpg",
                    Name = "Nike Jordans",
                    Price = 12
                }
            }.AsQueryable();
        }

        public static ProductCategory GetProductCategory()
        {
            return new ProductCategory
            {
                Id = "abc123",
                Category = "Sneakers"
            };
        }

        public static IQueryable<ProductCategory> GetProductCategories()
        {
            return new List<ProductCategory>
            {
                new ProductCategory
                {
                    Category = "Sneakers"
                },
                new ProductCategory
                {
                    Category = "T-Shirt"
                }
            }.AsQueryable();
        }
    }
}
