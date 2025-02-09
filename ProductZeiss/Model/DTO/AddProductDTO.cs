using System.ComponentModel.DataAnnotations;

namespace ProductZeissApi.Model.DTO
{
    public class AddProductDTO
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Name has to be Maximum of 50 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Description has to be Maximum of 100 characters")]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public int StockAvailable { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Category has to be Maximum of 50 characters")]
        public string Category { get; set; }
    }
}
