using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AutoMapper;
using ProductZeissApi.Controllers;
using ProductZeissApi.Data;
using ProductZeissApi.Model.Domain;
using ProductZeissApi.Model.DTO;
using ProductZeissApi.Repositories;

namespace ProductApi.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductDbContext _dbContext;
        private readonly ProductController _controller;

        public ProductsControllerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();

            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ProductDbContext(options);

            _controller = new ProductController(_dbContext, _mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateProduct_Test()
        {
            var productDto = new AddProductDTO
            {
                Name = "Test Product Creation",
                Description = "Creation of the product of UT",
                Price = 100,
                StockAvailable = 100,
                Category = "Test Category"
            };

            var product = new Product
            {
                ProductId = 123456,  // Random 6 digit Product ID
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                StockAvailable = productDto.StockAvailable,
                Category = productDto.Category,
                CreatedAt = DateTime.UtcNow
            };

            // Mock repository to accept any product and return it
            _mockRepository.Setup(r => r.CreateProductAsync(It.IsAny<Product>()))
                           .ReturnsAsync((Product p) => p);

            // Mock mapper for Product -> ProductDTO
            _mockMapper.Setup(m => m.Map<ProductDTO>(It.IsAny<Product>()))
                       .Returns((Product p) => new ProductDTO
                       {
                           ProductId = p.ProductId,
                           Name = p.Name,
                           Price = p.Price,
                           StockAvailable = p.StockAvailable,
                           Description = p.Description,
                           Category = p.Category
                       });
            var result = await _controller.CreateProduct(productDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<ProductDTO>(createdAtActionResult.Value);
            Assert.Equal("Test Product Creation", returnValue.Name);
            Assert.Equal(100, returnValue.StockAvailable);
        }


        [Fact]
        public async Task GetProductById_Test()
        {
            var product = new Product
            {
                ProductId = 112233,
                Name = "Existing Product",
                Price = 50,
                StockAvailable = 50,
                Description = "Test Description",
                Category = "Test Category"
            };

            _mockRepository.Setup(r => r.GetProductByIdAsync(112233)).ReturnsAsync(product);

            _mockMapper.Setup(m => m.Map<ProductDTO>(product)).Returns(new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                StockAvailable = product.StockAvailable,
                Description = product.Description,
                Category = product.Category
            });

            var result = await _controller.GetProductById(112233);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProductDTO>(okResult.Value);
            Assert.Equal("Existing Product", returnValue.Name);
            Assert.Equal(50, returnValue.StockAvailable);
        }

        [Fact]
        public async Task UpdateProduct_Test()
        {
            var updateDto = new UpdateProductDTO
            {
                Name = "Test Updated Name",
                StockAvailable = 25,
                Description = "Updated Description",
                Price = 100,
                Category = "Updated Category"
            };
            var productDomain = new Product
            {
                ProductId = 112233,
                Name = "Old Name Mock",
                Price = 50,
                StockAvailable = 20,
                Description = "Old Test Description",
                Category = "Old Test Category"
            };
            var updatedProduct = new Product
            {
                ProductId = 112233,
                Name = "Test Updated Name",
                Price = 100,
                StockAvailable = 25,
                Description = "Updated Description",
                Category = "Updated Category"
            };

            _mockMapper.Setup(m => m.Map<Product>(updateDto)).Returns(updatedProduct);
            _mockRepository.Setup(r => r.UpdateProductAsync(112233, updatedProduct)).ReturnsAsync(updatedProduct);
            _mockMapper.Setup(m => m.Map<ProductDTO>(updatedProduct)).Returns(new ProductDTO
            {
                ProductId = updatedProduct.ProductId,
                Name = updatedProduct.Name,
                StockAvailable = updatedProduct.StockAvailable,
                Category = updatedProduct.Category,
                Description = updatedProduct.Description,
                Price = updatedProduct.Price,
                CreatedAt = updatedProduct.CreatedAt,
                UpdatedAt = updatedProduct.UpdatedAt
            });

            var result = await _controller.UpdateProduct(112233, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<ProductDTO>(okResult.Value);
            Assert.Equal("Test Updated Name", returnedProduct.Name);
            Assert.Equal(25, returnedProduct.StockAvailable);
        }


        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_Test()
        {

            var product = new Product
            {
                ProductId = 123456,
                Name = "Existing Product",
                Price = 50,
                StockAvailable = 50,
                Description = "Test Description",
                Category = "Test Category"
            };

            _mockRepository.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.DeleteProductAsync(1)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductDTO>(product)).Returns(new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                StockAvailable = product.StockAvailable,
                Description = product.Description,
                Category = product.Category
            });

            var result = await _controller.DeleteProduct(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddToStock_Test()
        {
            var product = new Product
            {
                ProductId = 987654,
                Name = "Test_AddToStock",
                Price = 50,
                StockAvailable = 50,
                Description = "Test AddToStock Description",
                Category = "Test Category"
            };

            // Mocking UpdateAddToStockAsync to return the product with updated stock
            _mockRepository.Setup(r => r.UpdateAddToStockAsync(987654, 30))
                           .ReturnsAsync(new Product
                           {
                               ProductId = 987654,
                               Name = "Stock Product",
                               StockAvailable = 50, // 20 + 30
                               UpdatedAt = DateTime.UtcNow
                           });

            IActionResult result = await _controller.AddToStock(987654, 30);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualString = Assert.IsType<string>(okResult.Value);            
            Assert.Equal("Stock added successfully.", actualString);
            _mockRepository.Verify(r => r.UpdateAddToStockAsync(987654, 30), Times.Once);
        }      

    }
}
