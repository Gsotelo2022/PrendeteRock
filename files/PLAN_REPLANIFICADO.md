# Plan de Implementación Replanificado

## Situación Actual

✅ **Tienes:** Interfaces Vue.js completas  
❌ **No tienes:** Backend .NET funcional  
❌ **No tienes:** Integración Frontend-Backend  

Esto **cambia completamente el orden** de desarrollo.

---

## Nuevo Plan: 4 Fases

### FASE 1: Backend Base (2 semanas)

**Objetivo:** Tener un backend .NET funcional que pueda ser consumido por Vue.js

#### PASO 1.1: Models + DbContext (3-4 días)

Crear la base de datos en código C#:

```
/Models/
├─ User.cs
├─ Product.cs
├─ Order.cs
├─ OrderItem.cs
├─ Design.cs
└─ Coupon.cs

/Data/
└─ ApplicationDbContext.cs
```

**Salida:** Tablas en PostgreSQL, relaciones configuradas

#### PASO 1.2: DTOs + Program.cs (2-3 días)

Definir cómo se envía/recibe data en HTTP:

```
/DTOs/
├─ Auth/LoginRequest.cs, RegisterRequest.cs, AuthResponse.cs
├─ Products/ProductDTO.cs, CreateProductRequest.cs
├─ Orders/CreateOrderRequest.cs, OrderDTO.cs
└─ Images/GenerateImageRequest.cs

Program.cs: Configurar BD, CORS, Swagger, JWT
```

**Salida:** Swagger funcionando en `http://localhost:5000/swagger`

---

### FASE 2: Servicios + Controllers (3 semanas)

**Objetivo:** Endpoints REST listos para que Vue.js los consuma

#### PASO 2.1: Services Críticos (5-6 días)

Implementar la lógica de negocio:

```
/Services/
├─ IAuthService.cs / AuthService.cs
│  ├─ RegisterAsync()
│  ├─ LoginAsync()
│  └─ GenerateJWT()
│
├─ IProductService.cs / ProductService.cs
│  ├─ GetAllAsync()
│  ├─ GetByIdAsync(id)
│  ├─ CreateAsync()
│  ├─ UpdateAsync()
│  └─ DeleteAsync()
│
├─ IOrderService.cs / OrderService.cs
│  ├─ CreateAsync()
│  ├─ GetByUserAsync(userId)
│  └─ GetByIdAsync(id)
│
└─ DiscountService.cs
   └─ CalculateDiscount()
```

**Por qué AQUÍ:** Antes que Controllers para separar responsabilidades

#### PASO 2.2: Controllers (5-6 días)

Exponer endpoints REST:

```
/Controllers/
├─ AuthController.cs
│  ├─ POST /api/auth/register
│  └─ POST /api/auth/login
│
├─ ProductsController.cs
│  ├─ GET /api/products
│  ├─ GET /api/products/{id}
│  ├─ POST /api/products (admin)
│  ├─ PUT /api/products/{id} (admin)
│  └─ DELETE /api/products/{id} (admin)
│
├─ OrdersController.cs
│  ├─ POST /api/orders
│  ├─ GET /api/orders
│  └─ GET /api/orders/{id}
│
└─ AdminController.cs (básico)
```

**Salida:** Backend COMPLETO sin Ollama

---

### FASE 3: Integración Frontend-Backend (1-2 semanas)

**Objetivo:** Vue.js consumiendo APIs de .NET

#### PASO 3.1: Actualizar composables Vue.js

Cambiar dónde apuntan los requests HTTP:

```javascript
// Antes (FastAPI)
const baseURL = 'http://localhost:8000/api'

// Después (.NET)
const baseURL = 'http://localhost:5000/api'
```

#### PASO 3.2: Testing End-to-End

Validar flujos completos:
1. Registrarse → recibir JWT → guardar en localStorage ✓
2. Login → obtener JWT ✓
3. Ver productos ✓
4. Crear orden ✓
5. Ver órdenes propias ✓

**Salida:** Frontend + Backend integrados

---

### FASE 4: Features Avanzadas (2-3 semanas, opcional)

**Solo si el MVP básico funciona bien:**

#### PASO 4.1: Generación de Imágenes (Replicate)

```
/Services/
├─ IImageService.cs / ImageService.cs
│  └─ GenerateImageAsync(prompt)
│
└─ IRemoveBackgroundService.cs / RemoveBackgroundService.cs
   └─ RemoveBackgroundAsync(imageUrl)

/Controllers/
└─ ImagesController.cs
   ├─ POST /api/images/generate
   └─ POST /api/images/remove-background
```

#### PASO 4.2: Agregar Ollama (Fase 2 de Ollama)

```
/Services/
├─ OllamaService.cs
│  ├─ GenerateAsync(prompt)
│  └─ OptimizePromptAsync(prompt)
│
└─ ChatbotService.cs
   └─ ChatAsync(message)

/Controllers/
├─ DesignsController.cs (guardar diseños)
└─ ChatbotController.cs (responder preguntas)
```

---

## Ventaja: NO Repites Código

**Tu Vue.js YA TIENE:**
- Login.vue
- ProductsList.vue
- OrderForm.vue
- AdminPanel.vue
- Etc...

**Tú SOLO tienes que:**
- Cambiar las URLs en composables (de FastAPI a .NET)
- Los componentes siguen siendo los mismos
- Las funcionalidades siguen siendo las mismas

---

## Cronograma Realista

```
SEMANA 1:
├─ Día 1-2: Crear Models (User, Product, Order)
├─ Día 3-4: Crear DbContext + migrations
└─ Día 5: DTOs básicos

SEMANA 2:
├─ Día 1-2: Program.cs configurado
├─ Día 3-4: AuthService + AuthController (login/register)
└─ Día 5: ProductService + ProductsController

SEMANA 3:
├─ Día 1-2: OrderService + OrdersController
├─ Día 3-4: AdminController básico
└─ Día 5: Testing backend (Swagger manual)

SEMANA 4:
├─ Día 1-2: Actualizar composables Vue.js
├─ Día 3-4: Integración y testing end-to-end
└─ Día 5: Fixes y optimizaciones

RESULTADO: MVP Completo (sin Ollama)
```

---

## Qué NO Hacer

❌ **NO recrear componentes Vue.js**  
✓ Reutilizar los que ya tienes

❌ **NO duplicar lógica en múltiples sitios**  
✓ Lógica va en Services

❌ **NO agregar Ollama todavía**  
✓ Hacerlo después cuando MVP esté completo

❌ **NO hacer UI bonita en backend**  
✓ El backend es solo APIs, Vue.js es la UI

---

## Checklist para Empezar

```
□ Tengo .NET SDK 8 instalado
□ Tengo PostgreSQL corriendo
□ Tengo proyecto .NET creado
□ Tengo los 6 Models definidos
□ Tengo Models documentados (comentarios)
□ Tengo migrations creadas
□ Tengo tablas en BD

→ Si TODO = ✓, estás listo PASO 2
```

---

## Próximo Paso

1. Crear los Models (PASO 1.1)
2. Crear DbContext (PASO 1.1)
3. Reportar cuando estén listos

Después continuamos con DTOs (PASO 1.2)

---

*Plan replanificado - Mayo 2026*
