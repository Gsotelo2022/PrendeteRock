using Microsoft.EntityFrameworkCore;
using PrendeteRock.API.Data;
using PrendeteRock.API.Models;

namespace PrendeteRock.API.Services
{
    public class CouponService
    {
        private readonly ApplicationDbContext _context;

        public CouponService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Coupon>> GetAllAsync()
        {
            return await _context.Coupons.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<Coupon?> GetByIdAsync(int id)
        {
            return await _context.Coupons.FindAsync(id);
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower() && c.IsActive);
        }

        public async Task<Coupon> CreateAsync(string code, decimal percentage, DateTime? expiryDate, int? usageLimit)
        {
            try
            {
                var coupon = new Coupon
                {
                    Code = code.ToUpper(),
                    Percentage = percentage,
                    ExpiryDate = expiryDate.HasValue ? DateTime.SpecifyKind(expiryDate.Value, DateTimeKind.Utc) : null,
                    UsageLimit = usageLimit,
                    UsageCount = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();
                return coupon;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating coupon: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new Exception($"Error al crear cupón: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(int id, string code, decimal percentage, DateTime? expiryDate, int? usageLimit, bool isActive)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) throw new Exception("Cupón no existe");

            coupon.Code = code.ToUpper();
            coupon.Percentage = percentage;
            coupon.ExpiryDate = expiryDate.HasValue ? DateTime.SpecifyKind(expiryDate.Value, DateTimeKind.Utc) : null;
            coupon.UsageLimit = usageLimit;
            coupon.IsActive = isActive;

            await _context.SaveChangesAsync();
        }

        public async Task ToggleStatusAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) throw new Exception("Cupón no existe");

            coupon.IsActive = !coupon.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) throw new Exception("Cupón no existe");

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task IncrementUsageAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) throw new Exception("Cupón no existe");

            coupon.UsageCount++;
            await _context.SaveChangesAsync();
        }
    }
}
