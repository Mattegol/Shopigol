using Microsoft.EntityFrameworkCore;
using Shopigol.Core.Models;

namespace Shopigol.DataAccess.SQL
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }
    }
}
