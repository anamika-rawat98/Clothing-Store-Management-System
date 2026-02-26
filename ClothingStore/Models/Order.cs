using System.ComponentModel.DataAnnotations;

namespace ClothingStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;  // tell compiler it will be set

        [Required]
        public string Status { get; set; } = "Pending";

        [Range(0, 1000000)]
        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // initialize empty list
    }
}
