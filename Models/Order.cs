using System.ComponentModel.DataAnnotations;

namespace WinterSheltonHouse.Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public enum PaymentMethod
    {
        CashOnDelivery,
        CreditCard
    }

    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public PaymentMethod PaymentMethod { get; set; }

        public decimal TotalAmount { get; set; }

        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        public string ShippingCity { get; set; } = string.Empty;

        [Required]
        public string ShippingPhone { get; set; } = string.Empty;

        public string? CardLastFour { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
