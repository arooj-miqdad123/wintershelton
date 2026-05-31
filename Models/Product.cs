using System.ComponentModel.DataAnnotations;

namespace WinterSheltonHouse.Models
{
    public enum ProductCategory
    {
        Sweaters,
        Jackets,
        Gloves,
        Socks
    }

    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999)]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public ProductCategory Category { get; set; }

        public string ImageUrl { get; set; } = "/images/placeholder.jpg";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
