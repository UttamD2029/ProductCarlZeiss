using System.ComponentModel.DataAnnotations;

namespace ProductZeissApi.Model.DTO
{
    public class ProductDTO
    {

        public int ProductId { get; set; } // Auto-generated 6-digit unique ID
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        public int StockAvailable { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
