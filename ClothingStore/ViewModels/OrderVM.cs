using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClothingStore.ViewModels
{
    public class OrderVM
    {
        public int OrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        // Dropdowns
        public IEnumerable<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Products { get; set; } = new List<SelectListItem>();
    }
}
