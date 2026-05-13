namespace PrendeteRock.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountApplied { get; set; } = 0;
        public decimal FinalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        public string? PaymentId { get; set; }
        public string? CouponCodeUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public User? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}