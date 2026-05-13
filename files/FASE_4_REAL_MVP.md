# FASE 4 REAL: MVP Funcional 100%

## 🎯 Objetivo REAL de FASE 4

**No agregar features complejas. Solo asegurar que TODO funciona juntos.**

Tienes todo creado en FASE 1-3. Ahora:
1. Probar que todo funciona
2. Completar endpoints faltantes
3. Asegurar autenticación (JWT)
4. Validar flujos end-to-end

**Salida:** MVP operativo donde:
- ✅ Te registras
- ✅ Haces login
- ✅ Ves productos
- ✅ Creas órdenes
- ✅ Ves tus órdenes
- ✅ Admin gestiona todo

---

## 📋 Tareas Mínimas de FASE 4

### Tarea 4.1: Agregar JWT a AuthController

**El problema:** Hasta ahora AuthController retorna datos pero NO JWT.

**La solución:** Generar JWT tokens reales.

**Dónde:** Crear `/Helpers/JwtHelper.cs`

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AIPrintStudio.API.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(int userId, string email, bool isAdmin)
        {
            var jwtSecret = _config["Jwt:Secret"] ?? "default-secret-key-min-32-chars-long";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim("isAdmin", isAdmin.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "aiprintstudio",
                audience: _config["Jwt:Audience"] ?? "aiprintstudio-client",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
```

**Actualizar `appsettings.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=aiprintstudio;User Id=postgres;Password=tu_password;"
  },
  "Jwt": {
    "Secret": "tu-secret-key-debe-tener-minimo-32-caracteres-aqui-12345678901234567890",
    "Issuer": "aiprintstudio",
    "Audience": "aiprintstudio-client"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

**Actualizar `AuthService.cs`:**

```csharp
public async Task<AuthResponse> LoginAsync(string email, string password, JwtHelper jwtHelper)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    
    if (user == null || !VerifyPassword(password, user.PasswordHash))
        throw new UnauthorizedAccessException("Credenciales inválidas");

    // Generar JWT
    var token = jwtHelper.GenerateToken(user.Id, user.Email, user.IsAdmin);

    return new AuthResponse
    {
        Token = token,
        User = new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        }
    };
}

public async Task<AuthResponse> RegisterAsync(string email, string password, string fullName, JwtHelper jwtHelper)
{
    if (await _context.Users.AnyAsync(u => u.Email == email))
        throw new Exception("Email ya registrado");

    var user = new User
    {
        Email = email,
        FullName = fullName,
        PasswordHash = HashPassword(password),
        IsAdmin = false
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    var token = jwtHelper.GenerateToken(user.Id, user.Email, false);

    return new AuthResponse
    {
        Token = token,
        User = new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        }
    };
}
```

**Actualizar `AuthController.cs`:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtHelper _jwtHelper;

    public AuthController(AuthService authService, JwtHelper jwtHelper)
    {
        _authService = authService;
        _jwtHelper = jwtHelper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request.Email, request.Password, request.FullName, _jwtHelper);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request.Email, request.Password, _jwtHelper);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

**Registrar JwtHelper en `Program.cs`:**

```csharp
builder.Services.AddScoped<JwtHelper>();
```

---

### Tarea 4.2: Agregar [Authorize] donde se necesita

**El problema:** Cualquiera puede crear órdenes, cambiar status, etc. Sin JWT.

**La solución:** Proteger endpoints con `[Authorize]`

**En `OrdersController.cs`:**

```csharp
[Authorize]  // ← Agregar esto
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
{
    // ...
}

[Authorize]  // ← Agregar esto
[HttpGet("{userId}")]
public async Task<IActionResult> GetByUser(int userId)
{
    // ...
}
```

**En `DesignsController.cs`:**

```csharp
[Authorize]  // ← Agregar esto
[HttpPost]
public async Task<IActionResult> SaveDesign([FromBody] SaveDesignRequest request)
{
    // ...
}

[Authorize]  // ← Agregar esto
[HttpGet("user/{userId}")]
public async Task<IActionResult> GetUserDesigns(int userId)
{
    // ...
}

[Authorize]  // ← Agregar esto
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteDesign(int id)
{
    // ...
}
```

**En `AdminController.cs`:** (todos los métodos)

```csharp
[Authorize]  // ← Agregar aquí
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    // Todos los métodos ya están protegidos
}
```

---

### Tarea 4.3: Endpoints Mínimos Faltantes

**DesignsController completado:**

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIPrintStudio.API.Data;
using AIPrintStudio.API.Models;

namespace AIPrintStudio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DesignsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DesignsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveDesign([FromBody] SaveDesignRequest request)
        {
            try
            {
                var design = new Design
                {
                    UserId = request.UserId,
                    ImageUrl = request.ImageUrl,
                    PromptUsed = request.PromptUsed
                };

                _context.Designs.Add(design);
                await _context.SaveChangesAsync();

                return Ok(design);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDesigns(int userId)
        {
            try
            {
                var designs = await _context.Designs
                    .Where(d => d.UserId == userId)
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync();

                return Ok(designs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesign(int id)
        {
            try
            {
                var design = await _context.Designs.FindAsync(id);
                if (design == null)
                    return NotFound();

                _context.Designs.Remove(design);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Diseño eliminado" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class SaveDesignRequest
    {
        public int UserId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? PromptUsed { get; set; }
    }
}
```

**AdminController básico:**

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIPrintStudio.API.Data;
using AIPrintStudio.API.Models;

namespace AIPrintStudio.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ver todas las órdenes
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Cambiar estado de orden
        [HttpPatch("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                    return NotFound();

                order.Status = request.Status;
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Ver todos los cupones
        [HttpGet("coupons")]
        public async Task<IActionResult> GetCoupons()
        {
            try
            {
                var coupons = await _context.Coupons.ToListAsync();
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Crear cupón
        [HttpPost("coupons")]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequest request)
        {
            try
            {
                var coupon = new Coupon
                {
                    Code = request.Code.ToUpper(),
                    Percentage = request.Percentage,
                    ExpiryDate = request.ExpiryDate,
                    UsageLimit = request.UsageLimit
                };

                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();

                return Ok(coupon);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Ver todos los productos
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Ver todos los usuarios
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class CreateCouponRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
    }
}
```

---

### Tarea 4.4: Agregar DiscountService a OrderService

**El problema:** Cuando creo una orden, no calculo descuentos.

**La solución:** Usar DiscountService en OrderService.

**Actualizar `OrderService.cs`:**

```csharp
public class OrderService
{
    private readonly ApplicationDbContext _context;
    private readonly DiscountService _discountService;

    public OrderService(ApplicationDbContext context, DiscountService discountService)
    {
        _context = context;
        _discountService = discountService;
    }

    public async Task<Order> CreateAsync(int userId, int productId, int quantity, string? couponCode = null)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("Usuario no existe");

        var product = await _context.Products.FindAsync(productId);
        if (product == null) throw new Exception("Producto no existe");

        var order = new Order
        {
            UserId = userId,
            TotalPrice = product.BasePrice * quantity,
            Status = "Pending"
        };

        var orderItem = new OrderItem
        {
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = product.BasePrice
        };

        order.OrderItems.Add(orderItem);

        // Calcular descuento
        decimal discountPercentage = _discountService.CalculateDiscount(order, couponCode);
        order.DiscountApplied = order.TotalPrice * discountPercentage;
        order.FinalPrice = order.TotalPrice - order.DiscountApplied;
        order.CouponCodeUsed = couponCode;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    // Rest del código igual...
}
```

**Registrar en `Program.cs`:**

```csharp
builder.Services.AddScoped<DiscountService>();
```

---

### Tarea 4.5: Actualizar CreateOrderRequest

```csharp
public class CreateOrderRequest
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? CouponCode { get; set; }  // ← Agregar esto
}
```

---

## ✅ Checklist FASE 4 REAL

```
JWT Y AUTENTICACIÓN:
□ JwtHelper.cs creado
□ appsettings.json con Jwt config
□ AuthService retorna token
□ AuthController retorna token en login/register
□ [Authorize] en OrdersController
□ [Authorize] en DesignsController
□ [Authorize] en AdminController

ENDPOINTS:
□ POST /api/auth/login (retorna JWT)
□ POST /api/auth/register (retorna JWT)
□ POST /api/orders (crea orden con descuento)
□ GET /api/orders/{userId} (ver órdenes)
□ POST /api/designs (guardar diseño)
□ GET /api/designs/user/{userId}
□ DELETE /api/designs/{id}
□ GET /api/admin/orders
□ PATCH /api/admin/orders/{id}/status
□ GET /api/admin/coupons
□ POST /api/admin/coupons
□ GET /api/admin/products
□ GET /api/admin/users

VERIFICACIÓN:
□ dotnet build sin errores
□ dotnet run ejecuta sin errores
□ Swagger muestra todos endpoints
□ Probar login en Swagger (retorna token)
□ Probar crear orden con token
□ Probar endpoints admin con token
```

---

## 🎉 MVP LISTO

Después de FASE 4:
✅ Autenticación con JWT  
✅ Órdenes con descuentos  
✅ Galería de diseños  
✅ Panel admin  
✅ Todo protegido  

**Lo que hiciste en FASE 4 avanzada (Ollama, ImageService, etc.) lo usarás DESPUÉS si quieres mejorar.**

---

## ⚠️ Lo Que NO Hagas Ahora

❌ Integración con Replicate API  
❌ Integración con Ollama  
❌ Integración con Mercado Pago  
❌ Emails  
❌ Notificaciones  

**Todo eso es v1.1, después del MVP.**

---

*FASE 4 REAL - MVP 100% Funcional*
