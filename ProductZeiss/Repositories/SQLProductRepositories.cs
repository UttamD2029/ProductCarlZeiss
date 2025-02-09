using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using ProductZeissApi.Data;
using ProductZeissApi.Model.Domain;
using ProductZeissApi.Model.DTO;

namespace ProductZeissApi.Repositories
{
    public class SQLProductRepositories : IProductRepository
    {
        private readonly ProductDbContext dbContext;

        public SQLProductRepositories(ProductDbContext dbContext) 
        {
            this.dbContext = dbContext;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding product.", ex);
            }

        }

        public async Task<Product?> DeleteProductAsync(int id)
        {
            try
            {
                var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
                if (existingProduct == null)
                {
                    return null;
                }

                //Delete product
                dbContext.Products.Remove(existingProduct);
                await dbContext.SaveChangesAsync();
                return existingProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deleting the product.", ex);
            }
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            try
            {
                return await dbContext.Products.ToListAsync();
            }
            catch (Exception ex)                
            {
                throw new Exception("Error while fetching all products.", ex);
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                return await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching product by ID.", ex);
            }
        }

        public async Task<Product?> UpdateDecrementStockAsync(int id, int quantity)
        {
            try
            {
                var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
                if (existingProduct == null)
                {
                    return null;
                }

                if (quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero.");
                }

                if (existingProduct.StockAvailable < quantity)
                {
                    throw new InvalidOperationException("Insufficient stock available.");
                }

                existingProduct.StockAvailable -= quantity;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                var decrementedProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
                return decrementedProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while decrementing Stock Available.", ex);
            }
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            try
            {
                var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
                if (existingProduct == null)
                {
                    return null;
                }
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Category = product.Category;
                existingProduct.StockAvailable = product.StockAvailable;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
                return existingProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating the product.", ex);
            }
        }

        public async Task<Product?> UpdateAddToStockAsync(int id, int quantity)
        {
            try
            {
                var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
                if (existingProduct == null)
                {
                    return null;
                }

                if (quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero.");
                }

                existingProduct.StockAvailable += quantity;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                var addedProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
                return addedProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding the Stocks.", ex);
            }
        }

       
    }
}
