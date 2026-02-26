using System.ComponentModel.DataAnnotations;

namespace ClothingStore.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(1, 10000)]
        public decimal Price { get; set; }

        [Url]
        public string ImageUrl { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
