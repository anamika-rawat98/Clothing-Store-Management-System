using System.ComponentModel.DataAnnotations;

namespace ClothingStore.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
