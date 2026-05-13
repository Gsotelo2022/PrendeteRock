# FASE 2: Servicios + Controllers (Semanas 2-3)

## ðŸŽ¯ Objetivo de esta Fase

Crear endpoints (URLs) que tu Vue.js pueda consumir.

**Salida:** Backend con endpoints como:
- `POST /api/auth/login`
- `GET /api/products`
- `POST /api/orders`
- Etc...

---

## ðŸ“‹ Tareas de FASE 2

### Tarea 2.1: Crear Services (LÃ³gica de Negocio)

**Â¿QuÃ© es?** Archivos C# que contienen la lÃ³gica (no la interfaz).

**DÃ³nde?** Carpeta `/Services/`

#### AuthService.cs

```csharp
using PrendeteRock.API.Data;
using PrendeteRock.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace PrendeteRock.API.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> RegisterAsync(string email, string password, string fullName)
        {
            // Â¿Ya existe el email?
            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new Exception("Email ya registrado");

            // Crear usuario
            var user = new User
            {
                Email = email,
                FullName = fullName,
                PasswordHash = HashPassword(password)
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
    }
}
```

#### ProductService.cs

```csharp
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

        public async Task<Product> CreateAsync(string name, string description, decimal basePrice, string category)
        {
            var product = new Product
            {
                Name = name,
                Description = description,
                BasePrice = basePrice,
                Category = category
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(int id, string name, string description, decimal basePrice)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new Exception("Producto no existe");

            product.Name = name;
            product.Description = description;
            product.BasePrice = basePrice;

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
```

#### OrderService.cs

```csharp
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
```

#### DiscountService.cs

```csharp
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

            // Descuento por cupÃ³n
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = _context.Coupons
                    .FirstOrDefault(c => c.Code == couponCode && c.IsActive);

                if (coupon != null && (coupon.ExpiryDate == null || coupon.ExpiryDate > DateTime.UtcNow))
                {
                    discount += coupon.Percentage / 100;
                }
            }

            // MÃ¡ximo 35%
            return Math.Min(discount, 0.35m);
        }
    }
}
```

---

### Tarea 2.2: Registrar Services en Program.cs

En `Program.cs`, ANTES de `var app = builder.Build();`:

```csharp
// Agregar Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<DiscountService>();
```

---

### Tarea 2.3: Crear Controllers (Endpoints)

**Â¿QuÃ© es?** Archivos que definen las URLs que Vue.js va a llamar.

**DÃ³nde?** Carpeta `/Controllers/`

#### AuthController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using PrendeteRock.API.Services;

namespace PrendeteRock.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request.Email, request.Password, request.FullName);
                return Ok(new { message = "Usuario registrado", userId = user.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authService.LoginAsync(request.Email, request.Password);
                if (user == null)
                    return Unauthorized("Credenciales invÃ¡lidas");

                return Ok(new { message = "Login exitoso", userId = user.Id, email = user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
```

#### ProductsController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using PrendeteRock.API.Services;
using PrendeteRock.API.Models;

namespace PrendeteRock.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                var product = await _productService.CreateAsync(
                    request.Name, request.Description, request.BasePrice, request.Category);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
```

#### OrdersController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using PrendeteRock.API.Services;

namespace PrendeteRock.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            try
            {
                var order = await _orderService.CreateAsync(request.UserId, request.ProductId, request.Quantity);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var orders = await _orderService.GetByUserAsync(userId);
            return Ok(orders);
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }
    }

    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
```

---

## âœ?Checklist FASE 2

```
â–?AuthService creado
â–?ProductService creado
â–?OrderService creado
â–?DiscountService creado
â–?Services registrados en Program.cs
â–?AuthController creado
â–?ProductsController creado
â–?OrdersController creado
â–?dotnet build sin errores
â–?Swagger muestra endpoints: /api/auth/login, /api/products, etc
â–?Puedes probar endpoints en Swagger (Try it out)
```

**Cuando TODO estÃ© âœ? pasas a FASE 3.**

---

*FASE 2 - Servicios y Controllers - GuÃ­a Completa para Principiantes*
