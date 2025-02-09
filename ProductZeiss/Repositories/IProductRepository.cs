using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using ProductZeissApi.Model.Domain;

namespace ProductZeissApi.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductAsync();

        Task<Product?> GetProductByIdAsync(int id);

        Task<Product> CreateProductAsync(Product product);

        Task<Product?> UpdateProductAsync(int id, Product product);

        Task<Product?> DeleteProductAsync(int id);

        Task<Product?> UpdateDecrementStockAsync(int id, int quantity);

        Task<Product?> UpdateAddToStockAsync(int id, int quantity);
    }
}
