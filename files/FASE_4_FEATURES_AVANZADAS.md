# FASE 4: Features Avanzadas (Semanas 4-5)

## 🎯 Objetivo de esta Fase

Agregar funcionalidades opcionales al MVP:
- Generación de imágenes con Ollama
- Galería de diseños
- Endpoints de admin avanzados

**Salida:** MVP COMPLETO con todas las funcionalidades.

---

## 📋 Tareas de FASE 4

### Tarea 4.1: Instalar Ollama (Día 1)

**Paso 1: Descargar Ollama**
1. Ve a https://ollama.com
2. Descarga e instala para tu sistema operativo

**Paso 2: Instalar modelo**
```bash
ollama pull qwen2.5
```

**Paso 3: Ejecutar Ollama**
```bash
ollama serve
```
Debería estar en `http://localhost:11434`

---

### Tarea 4.2: Crear OllamaService (Día 2)

**Dónde:** `/Services/OllamaService.cs`

```csharp
using System.Net.Http.Json;

namespace AIPrintStudio.API.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _ollamaUrl = "http://localhost:11434";

        public OllamaService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GenerateAsync(string prompt, string model = "qwen2.5")
        {
            try
            {
                var request = new
                {
                    model = model,
                    prompt = prompt,
                    stream = false
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_ollamaUrl}/api/generate",
                    request
                );

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error en Ollama");

                var result = await response.Content.ReadAsAsync<dynamic>();
                return result["response"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en Ollama: {ex.Message}");
            }
        }

        public async Task<string> OptimizePromptAsync(string prompt)
        {
            // Usar Ollama para mejorar el prompt
            string optimizationPrompt = $@"
Eres un experto en crear prompts para generadores de imágenes.
Toma este prompt y mejóralo para obtener mejores resultados:

Prompt original: {prompt}

Responde SOLO con el prompt mejorado, sin explicaciones.
";

            return await GenerateAsync(optimizationPrompt);
        }
    }
}
```

---

### Tarea 4.3: Crear ImageService (Día 2)

**Dónde:** `/Services/ImageService.cs`

**Nota:** Esto requiere una API key de Replicate. Por ahora, es un placeholder.

```csharp
namespace AIPrintStudio.API.Services
{
    public class ImageService
    {
        private readonly OllamaService _ollamaService;
        private readonly HttpClient _httpClient;

        public ImageService(OllamaService ollamaService)
        {
            _ollamaService = ollamaService;
            _httpClient = new HttpClient();
        }

        public async Task<string> GenerateImageAsync(string prompt)
        {
            try
            {
                // Primero, optimizar el prompt con Ollama
                string optimizedPrompt = await _ollamaService.OptimizePromptAsync(prompt);
                
                // Aquí irían las llamadas a Replicate API
                // Por ahora, retornamos un placeholder
                return "https://placeholder-image.com/image.jpg";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar imagen: {ex.Message}");
            }
        }
    }
}
```

---

### Tarea 4.4: Crear ImagesController (Día 3)

**Dónde:** `/Controllers/ImagesController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using AIPrintStudio.API.Services;

namespace AIPrintStudio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImagesController(ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateImage([FromBody] GenerateImageRequest request)
        {
            try
            {
                var imageUrl = await _imageService.GenerateImageAsync(request.Prompt);
                return Ok(new { imageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class GenerateImageRequest
    {
        public string Prompt { get; set; } = string.Empty;
    }
}
```

---

### Tarea 4.5: Crear DesignsController (Día 3)

**Dónde:** `/Controllers/DesignsController.cs`

```csharp
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

        [HttpGet("{userId}")]
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

                return Ok("Diseño eliminado");
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

---

### Tarea 4.6: Expandir AdminController (Día 4)

**Dónde:** `/Controllers/AdminController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using AIPrintStudio.API.Data;
using AIPrintStudio.API.Models;

namespace AIPrintStudio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET all orders (admin)
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

        // UPDATE order status
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

        // GET all users (admin)
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
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

        // GET all coupons
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

        // CREATE coupon
        [HttpPost("coupons")]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequest request)
        {
            try
            {
                var coupon = new Coupon
                {
                    Code = request.Code,
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

### Tarea 4.7: Registrar Services en Program.cs

En `Program.cs`, agregar:

```csharp
builder.Services.AddScoped<OllamaService>();
builder.Services.AddScoped<ImageService>();
```

---

## ✅ Checklist FASE 4

```
□ Ollama instalado y ejecutándose
□ OllamaService creado
□ ImageService creado
□ ImagesController creado
□ DesignsController creado
□ AdminController expandido
□ Services registrados en Program.cs
□ dotnet build sin errores
□ Swagger muestra nuevos endpoints
□ Probar: POST /api/images/generate
□ Probar: POST /api/designs
□ Probar: GET /api/admin/orders
□ Probar: POST /api/admin/coupons
```

**Cuando TODO esté ✓, MVP COMPLETO.**

---

## 🎉 ¡MVP LISTO!

Tu aplicación tiene:
✅ Frontend Vue.js profesional  
✅ Backend .NET con APIs  
✅ Base de datos PostgreSQL  
✅ Autenticación funcional  
✅ Carrito de compras  
✅ Generación de imágenes con IA  
✅ Panel de administración  

---

## 📈 Próximos Pasos (Después del MVP)

Si quieres mejorar más:
1. Integración con Mercado Pago para pagos reales
2. Notificaciones por email
3. Búsqueda avanzada
4. Reviews de productos
5. Desplegar a producción

---

*FASE 4 - Features Avanzadas - Guía Completa para Principiantes*
