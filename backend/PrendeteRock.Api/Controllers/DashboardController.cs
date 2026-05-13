using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrendeteRock.API.Data;

namespace PrendeteRock.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics()
        {
            try
            {
                // Total de ingresos
                var totalRevenue = await _context.Orders.SumAsync(o => o.FinalPrice);

                // Total de pedidos
                var totalOrders = await _context.Orders.CountAsync();

                // Total de clientes (usuarios no admin)
                var totalCustomers = await _context.Users.CountAsync(u => !u.IsAdmin);

                // Total de productos activos
                var totalActiveProducts = await _context.Products.CountAsync(p => p.IsActive);

                // Pedidos recientes (últimos 10)
                var recentOrders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .Select(o => new
                    {
                        o.Id,
                        OrderId = "ORD-" + o.Id.ToString("D3"),
                        Customer = o.User != null ? o.User.FullName : "N/A",
                        Product = o.OrderItems.FirstOrDefault() != null && o.OrderItems.FirstOrDefault()!.Product != null 
                            ? o.OrderItems.FirstOrDefault()!.Product!.Name 
                            : "N/A",
                        Amount = o.FinalPrice,
                        o.Status,
                        o.CreatedAt
                    })
                    .ToListAsync();

                // Ingresos por mes (últimos 6 meses)
                var sixMonthsAgo = DateTime.Now.AddMonths(-5);
                var revenueByMonth = await _context.Orders
                    .Where(o => o.CreatedAt >= sixMonthsAgo)
                    .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Revenue = g.Sum(o => o.FinalPrice)
                    })
                    .OrderBy(g => g.Year).ThenBy(g => g.Month)
                    .ToListAsync();

                // Pedidos por mes (últimos 6 meses)
                var ordersByMonth = await _context.Orders
                    .Where(o => o.CreatedAt >= sixMonthsAgo)
                    .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Year).ThenBy(g => g.Month)
                    .ToListAsync();

                return Ok(new
                {
                    totalRevenue,
                    totalOrders,
                    totalCustomers,
                    totalActiveProducts,
                    recentOrders,
                    revenueByMonth,
                    ordersByMonth
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener métricas", error = ex.Message });
            }
        }
    }
}
