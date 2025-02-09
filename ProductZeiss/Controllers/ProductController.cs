using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductZeissApi.CustomActionFilters;
using ProductZeissApi.Data;
using ProductZeissApi.Model.Domain;
using ProductZeissApi.Model.DTO;
using ProductZeissApi.Repositories;

namespace ProductZeissApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext dbContext;
        private readonly IProductRepository productRepository;
        private static readonly Random _random = new Random();
        private readonly IMapper mapper;
        public ProductController(ProductDbContext productDbContext, IProductRepository productRepository,IMapper mapper)
        {
            this.dbContext = productDbContext;
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        private int GenerateUniqueProductId()
        {
            int newId;
            do
            {
                newId = _random.Next(100000, 1000000);
            } while (dbContext.Products.Any(p => p.ProductId == newId));

            return newId;
        }

        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAllProduct()
        {
            try
            {
                //Getting Data from DataBase in Domain models
                var productsDomain = await productRepository.GetAllProductAsync();

                //Mapping Domain to DTO's                
                var productsDto = mapper.Map<List<ProductDTO>>(productsDomain);

                return Ok(productsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            try
            {
                //Getting Data from DataBase in Domain models
                var productsDomain = await productRepository.GetProductByIdAsync(id);
                if (productsDomain == null)
                {
                    return NotFound();
                }
                //Mapping Domain to DTO's  
                var productDto = mapper.Map<ProductDTO>(productsDomain);
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> CreateProduct([FromBody] AddProductDTO addProductDTO)
        {
            try
            {
                //Mapping the DTO to Domain Model
                var addProductDomain = new Product
                {
                    ProductId = GenerateUniqueProductId(),
                    Name = addProductDTO.Name,
                    Description = addProductDTO.Description,
                    Price = addProductDTO.Price,
                    StockAvailable = addProductDTO.StockAvailable,
                    Category = addProductDTO.Category,
                    CreatedAt = DateTime.UtcNow
                };

                //Add the Domain to create a Product
                addProductDomain = await productRepository.CreateProductAsync(addProductDomain);
                
                var productDTO = mapper.Map<ProductDTO>(addProductDomain);

                //return CreatedAtAction("success", addProductDomain);
                return CreatedAtAction(nameof(GetProductById), new { id = productDTO.ProductId }, productDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        [Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id,
            [FromBody] UpdateProductDTO updateProductDTO)
        {
            try
            {                
                //Mapping DTO to Domain Model
                var productDomainModel = mapper.Map<Product>(updateProductDTO);

                productDomainModel = await productRepository.UpdateProductAsync(id, productDomainModel);
                if (productDomainModel == null)
                {
                    return NotFound();
                }

                // Convert back to DTO
                var productDTO = mapper.Map<ProductDTO>(productDomainModel);

                return Ok(productDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            try 
            { 
            var productDomainModel = await productRepository.DeleteProductAsync(id);
            if (productDomainModel == null)
            {
                return NotFound();
            }

            // Convert Domain to DTO model
            var productDTO = mapper.Map<ProductDTO>(productDomainModel);
            return Ok(productDTO);
            }
              catch (Exception ex)
            {
               return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut]
        [Route("decrement-stock/{id:int}/{quantity:int}")]
        [Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> DecrementStock(int id, int quantity)
        {
            try
            {
                var productDomainModel = await productRepository.UpdateDecrementStockAsync(id, quantity);
                if (productDomainModel == null)
                {
                    return NotFound();
                }

                return Ok(new { message = "Stock decremented successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPut]
        [Route("add-to-stock/{id:int}/{quantity:int}")]
        [Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> AddToStock(int id, int quantity)
        {
            try
            {
                var productDomainModel = await productRepository.UpdateAddToStockAsync(id, quantity);
                if (productDomainModel == null)
                {
                    return NotFound();
                }

                return Ok(new { message = "Stock added successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
