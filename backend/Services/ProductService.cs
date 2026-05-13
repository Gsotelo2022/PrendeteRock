using Microsoft.EntityFrameworkCore;
using PrendeteRock.API.Data;
using PrendeteRock.API.Models;

namespace PrendeteRock.API.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateAsync(string name, string description, decimal basePrice, string category, string? imageUrl, bool isActive, int stock)
        {
            var product = new Product
            {
                Name = name,
                Description = description,
                BasePrice = basePrice,
                Category = category,
                ImageUrl = imageUrl,
                IsActive = isActive,
                Stock = stock
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(int id, string name, string description, decimal basePrice, string category, string? imageUrl, bool isActive, int stock)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new Exception("Producto no existe");

            product.Name = name;
            product.Description = description;
            product.BasePrice = basePrice;
            product.Category = category;
            product.ImageUrl = imageUrl;
            product.IsActive = isActive;
            product.Stock = stock;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new Exception("Producto no existe");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}