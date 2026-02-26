using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClothingStore.Models
{
    public class Brand
    {
        public int BrandId { get; set; }

        [Required]
        public string Name { get; set; } = null!;  // initialize to avoid null error

        public string Country { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1800, 2100)]
        public int FoundedYear { get; set; }

        // Initialize collection to avoid null errors
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
