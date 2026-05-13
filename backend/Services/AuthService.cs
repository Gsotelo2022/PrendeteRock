using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrendeteRock.API.Data;
using PrendeteRock.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PrendeteRock.API.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User?> RegisterAsync(string email, string password, string fullName)
        {
            // ¿Ya existe el email?
            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new Exception("Email ya registrado");

            // Crear usuario
            var user = new User
            {
                Email = email,
                FullName = fullName,
                PasswordHash = HashPassword(password),
                // El primer usuario es admin
                IsAdmin = !await _context.Users.AnyAsync()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash);
        }

        // Generar JWT Token
        public string GenerateToken(User user)
        {
            var jwtSecret = _configuration["Jwt:Secret"] ?? "mysupersecretkeythatneedstobeatleast32characterslong";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("fullname", user.FullName),
                new Claim("isadmin", user.IsAdmin.ToString().ToLower())
            };

            var token = new JwtSecurityToken(
                issuer: "prendeterock",
                audience: "prendeterock-users",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}