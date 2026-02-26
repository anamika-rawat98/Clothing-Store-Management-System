using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClothingStore.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }  // For Edit/Delete
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
