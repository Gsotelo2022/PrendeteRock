using Microsoft.EntityFrameworkCore;
using PrendeteRock.API.Data;
using PrendeteRock.API.Models;

namespace PrendeteRock.API.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(int userId, int productId, int quantity)
        {
            // Validar usuario
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("Usuario no existe");

            // Validar producto
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new Exception("Producto no existe");

            // Crear orden
            var order = new Order
            {
                UserId = userId,
                TotalPrice = product.BasePrice * quantity,
                Status = "Pending"
            };

            // Crear order item
            var orderItem = new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.BasePrice
            };

            order.OrderItems.Add(orderItem);
            order.FinalPrice = order.TotalPrice;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>> GetByUserAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}