using Microsoft.EntityFrameworkCore;
using PrendeteRock.API.Data;
using PrendeteRock.API.Models;

namespace PrendeteRock.API.Services
{
    public class DiscountService
    {
        private readonly ApplicationDbContext _context;

        public DiscountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public decimal CalculateDiscount(Order order, string? couponCode)
        {
            decimal discount = 0;

            // Descuento por cantidad
            int totalQuantity = order.OrderItems.Sum(oi => oi.Quantity);
            if (totalQuantity >= 10)
                discount += 0.10m; // 10%
            if (totalQuantity >= 50)
                discount += 0.15m; // 15%

            // Descuento por cup贸n
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = _context.Coupons
                    .FirstOrDefault(c => c.Code == couponCode && c.IsActive);

                if (coupon != null && (coupon.ExpiryDate == null || coupon.ExpiryDate > DateTime.UtcNow))
                {
                    discount += coupon.Percentage / 100;
                }
            }

            // M谩ximo 35%
            return Math.Min(discount, 0.35m);
        }
    }
}