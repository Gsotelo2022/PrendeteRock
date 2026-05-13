# FASE 1: Backend Base (Semanas 1-2)

## 🎯 Objetivo de esta Fase

Tener un backend .NET que:
- Conecte a PostgreSQL
- Tenga todos los Models definidos
- Esté documentado en Swagger

**Salida:** Acceder a `http://localhost:5000/swagger` y ver todos los endpoints vacíos pero documentados.

---

## 📋 Tareas de FASE 1

### Tarea 1.1: Crear los 6 Models (Días 1-2)

**¿Qué es?** Archivos C# que representan tus tablas de BD.

**Dónde?** Carpeta `/Models/` en tu proyecto

**Los 6 Models:**
1. `User.cs` - Usuarios
2. `Product.cs` - Productos
3. `Order.cs` - Órdenes
4. `OrderItem.cs` - Items en órdenes
5. `Design.cs` - Diseños guardados
6. `Coupon.cs` - Cupones

**Código de User.cs:**
```csharp
namespace PrendeteRock.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Design> Designs { get; set; } = new List<Design>();
    }
}
```

**Código de Product.cs:**
```csharp
namespace PrendeteRock.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
```

**Código de Order.cs:**
```csharp
namespace PrendeteRock.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountApplied { get; set; } = 0;
        public decimal FinalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        public string? PaymentId { get; set; }
        public string? CouponCodeUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public User? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
```

**Código de OrderItem.cs:**
```csharp
namespace PrendeteRock.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
```

**Código de Design.cs:**
```csharp
namespace PrendeteRock.API.Models
{
    public class Design
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? PromptUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public User? User { get; set; }
    }
}
```

**Código de Coupon.cs:**
```csharp
namespace PrendeteRock.API.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
```

**Verificar:**
```bash
dotnet build
```
Debería compilar sin errores.

---

### Tarea 1.2: Crear ApplicationDbContext (Día 3)

**¿Qué es?** Archivo que conecta los Models con PostgreSQL.

**Dónde?** Carpeta `/Data/ApplicationDbContext.cs`

**Código completo:**
```csharp
using Microsoft.EntityFrameworkCore;
using PrendeteRock.API.Models;

namespace PrendeteRock.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Design> Designs { get; set; } = null!;
        public DbSet<Coupon> Coupons { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Email único
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Código de cupón único
            modelBuilder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique();

            // Relaciones
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<Design>()
                .HasOne(d => d.User)
                .WithMany(u => u.Designs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
```

**Verificar:**
```bash
dotnet build
```

---

### Tarea 1.3: Configurar appsettings.json (Día 4)

**¿Qué es?** Archivo que guarda la contraseña de PostgreSQL.

**Dónde?** Raíz del proyecto `/appsettings.json`

**Código:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=PrendeteRock;User Id=postgres;Password=tu_password;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

**IMPORTANTE:** Reemplaza `tu_password` con la contraseña que pusiste cuando instalaste PostgreSQL.

---

### Tarea 1.4: Configurar Program.cs (Día 4-5)

**¿Qué es?** Archivo principal que configura todo el backend.

**Dónde?** Raíz del proyecto `/Program.cs`

**Código:**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using PrendeteRock.API.Data;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Agregar DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Agregar controladores
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "AI Print Studio API", 
        Version = "v1" 
    });
});

// CORS (permite que Vue.js acceda)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()
    );
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

### Tarea 1.5: Crear Migraciones (Día 5)

**¿Qué es?** Crear las tablas en PostgreSQL.

**Terminal:**
```bash
cd PrendeteRock.API

# Crear migración
dotnet ef migrations add InitialCreate

# Aplicar a base de datos
dotnet ef database update
```

**¿Qué pasa?**
- Se crea carpeta `/Migrations/`
- Se crean tablas en PostgreSQL
- Verifica en pgAdmin que existan las 6 tablas

---

## �?Checklist FASE 1

```
�?6 Models creados (.cs files)
�?ApplicationDbContext creado
�?appsettings.json configurado
�?Program.cs configurado
�?Migraciones creadas
�?Tablas en PostgreSQL
�?dotnet build sin errores
�?Pueden ver: C:\PrendeteRock\backend\PrendeteRock.Api> dotnet run -> http://localhost:5000/swagger
```

**Cuando TODO esté �? pasas a FASE 2.**

---

*FASE 1 - Backend Base - Guía Completa para Principiantes*
