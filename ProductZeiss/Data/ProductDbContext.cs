using Microsoft.EntityFrameworkCore;
using ProductZeissApi.Model.Domain;

namespace ProductZeissApi.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> DbContextOptions) : base(DbContextOptions)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
