using Microsoft.EntityFrameworkCore;
using PrendeteRock.API.Data;
using PrendeteRock.API.Models;

namespace PrendeteRock.API.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios/clientes con estadísticas
        public async Task<List<UserWithStatsDto>> GetAllUsersWithStatsAsync()
        {
            var users = await _context.Users
                .Include(u => u.Orders)
                .ToListAsync();

            var result = users.Select(u => new UserWithStatsDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                IsAdmin = u.IsAdmin,
                CreatedAt = u.CreatedAt,
                TotalOrders = u.Orders.Count,
                TotalSpent = u.Orders.Sum(o => o.FinalPrice)
            }).ToList();

            return result;
        }

        // Obtener solo clientes (no admins) con estadísticas
        public async Task<List<UserWithStatsDto>> GetClientsWithStatsAsync()
        {
            var clients = await _context.Users
                .Where(u => !u.IsAdmin)
                .Include(u => u.Orders)
                .ToListAsync();

            var result = clients.Select(u => new UserWithStatsDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                IsAdmin = u.IsAdmin,
                CreatedAt = u.CreatedAt,
                TotalOrders = u.Orders.Count,
                TotalSpent = u.Orders.Sum(o => o.FinalPrice)
            }).ToList();

            return result;
        }

        // Obtener un usuario por ID con estadísticas
        public async Task<UserWithStatsDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            var result = new UserWithStatsDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt,
                TotalOrders = user.Orders.Count,
                TotalSpent = user.Orders.Sum(o => o.FinalPrice)
            };

            return result;
        }

        // Eliminar usuario
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Actualizar información del usuario
        public async Task<User?> UpdateUserAsync(int userId, string? fullName, string? email, string? phone)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return null;

            if (!string.IsNullOrEmpty(fullName))
                user.FullName = fullName;

            if (!string.IsNullOrEmpty(email))
                user.Email = email;

            if (phone != null)
                user.Phone = phone;

            await _context.SaveChangesAsync();
            return user;
        }
    }

    // DTO para incluir estadísticas
    public class UserWithStatsDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
