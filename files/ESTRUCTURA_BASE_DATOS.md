# Estructura de Base de Datos Ideal para MVP

## Filosofía de Diseño

**KISS: Keep It Simple, Stupid**

Solo lo necesario para:
- ✅ Registrarse y loguearse
- ✅ Ver y comprar productos
- ✅ Crear y ver órdenes
- ✅ Guardar diseños
- ✅ Aplicar descuentos
- ❌ NO: Inventario, shipping, facturación

---

## Diagrama Relacional

```
USERS (1)
    ├─── Orders (N)
    │     └─── OrderItems (N)
    │           └─── Products (N)
    │
    └─── Designs (N)


PRODUCTS (1)
    └─── OrderItems (N)
         └─── Orders (N)


COUPONS (independiente)
    └─ Usados en Orders
```

---

## Tabla por Tabla

### 1. USERS (Usuarios)

**Propósito:** Almacenar credenciales e información de usuario

```sql
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(MAX) NOT NULL,
    full_name VARCHAR(255),
    is_admin BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índice para búsquedas rápidas por email
CREATE INDEX idx_users_email ON users(email);
```

**Campos:**
- `id`: PK, auto-incremento
- `email`: Único, login
- `password_hash`: NUNCA contraseña plana, hashear con SHA256
- `full_name`: Nombre para mostrar
- `is_admin`: ¿puede acceder admin panel?
- `created_at`: Timestamp de creación

**Relaciones:**
- 1 User → N Orders
- 1 User → N Designs

---

### 2. PRODUCTS (Productos)

**Propósito:** Catálogo de productos que se pueden personalizar

```sql
CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    base_price DECIMAL(10, 2) NOT NULL,
    category VARCHAR(100),
    image_url VARCHAR(MAX),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índice para filtrar por categoría
CREATE INDEX idx_products_category ON products(category);
CREATE INDEX idx_products_active ON products(is_active);
```

**Campos:**
- `id`: PK
- `name`: "Remera básica", "Taza personalizada"
- `description`: Detalles del producto
- `base_price`: Precio base antes de descuentos
- `category`: "Clothing", "Mug", "Hoodie"
- `image_url`: Link a foto
- `is_active`: ¿visible en tienda?
- `created_at`: Cuándo se agregó

**Relaciones:**
- 1 Product → N OrderItems

---

### 3. ORDERS (Órdenes/Pedidos)

**Propósito:** Registro de todas las compras

```sql
CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES users(id) ON DELETE CASCADE,
    total_price DECIMAL(10, 2) NOT NULL,
    discount_applied DECIMAL(10, 2) DEFAULT 0,
    final_price DECIMAL(10, 2) NOT NULL,
    status VARCHAR(50) DEFAULT 'Pending',
    payment_id VARCHAR(255),
    coupon_code_used VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índices para búsquedas comunes
CREATE INDEX idx_orders_user ON orders(user_id);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_created ON orders(created_at);
```

**Campos:**
- `id`: PK
- `user_id`: FK a Users (propietario de la orden)
- `total_price`: Suma de OrderItems antes de descuentos
- `discount_applied`: Monto del descuento (ej: 10.50)
- `final_price`: Precio a pagar = total_price - discount_applied
- `status`: "Pending" → "Paid" → "Shipped" → "Delivered"
- `payment_id`: ID de Mercado Pago (cuando se paga)
- `coupon_code_used`: Código del cupón si se usó
- `created_at`: Cuándo se creó la orden
- `updated_at`: Última actualización

**Estados válidos:**
```
Pending    → Orden creada, no pagada
Paid       → Pago confirmado
Shipped    → En camino
Delivered  → Recibida
Cancelled  → Cancelada
```

**Relaciones:**
- N Orders → 1 User
- 1 Order → N OrderItems

---

### 4. ORDER_ITEMS (Items de la orden)

**Propósito:** Detallar qué productos hay en cada orden

```sql
CREATE TABLE order_items (
    id SERIAL PRIMARY KEY,
    order_id INT NOT NULL FOREIGN KEY REFERENCES orders(id) ON DELETE CASCADE,
    product_id INT NOT NULL FOREIGN KEY REFERENCES products(id),
    quantity INT NOT NULL DEFAULT 1,
    unit_price DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índices
CREATE INDEX idx_order_items_order ON order_items(order_id);
CREATE INDEX idx_order_items_product ON order_items(product_id);
```

**Campos:**
- `id`: PK
- `order_id`: FK a Orders
- `product_id`: FK a Products (qué se compra)
- `quantity`: Cuántos (ej: 5 remeras)
- `unit_price`: Precio unitario cuando se compró (historial)
- `created_at`: Timestamp

**Nota:** `unit_price` DEBE guardarse porque el precio del producto puede cambiar, pero el orden debe recordar qué costó en el momento.

**Relaciones:**
- N OrderItems → 1 Order
- N OrderItems → 1 Product

---

### 5. DESIGNS (Diseños guardados)

**Propósito:** Galería personal de diseños que el usuario generó con IA

```sql
CREATE TABLE designs (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES users(id) ON DELETE CASCADE,
    image_url VARCHAR(MAX) NOT NULL,
    prompt_used TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índice para buscar diseños de un usuario
CREATE INDEX idx_designs_user ON designs(user_id);
CREATE INDEX idx_designs_created ON designs(created_at);
```

**Campos:**
- `id`: PK
- `user_id`: FK a Users (quién lo creó)
- `image_url`: Link a la imagen generada
- `prompt_used`: El prompt que se usó para generarla
- `created_at`: Cuándo se generó

**Relaciones:**
- N Designs → 1 User

---

### 6. COUPONS (Cupones/Códigos de descuento)

**Propósito:** Gestionar códigos promocionales

```sql
CREATE TABLE coupons (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    percentage DECIMAL(5, 2) NOT NULL,
    expiry_date TIMESTAMP,
    usage_limit INT,
    usage_count INT DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índice para búsqueda rápida por código
CREATE INDEX idx_coupons_code ON coupons(code);
CREATE INDEX idx_coupons_active ON coupons(is_active);
```

**Campos:**
- `id`: PK
- `code`: "PROMO10", "SUMMER20" (único)
- `percentage`: 10 (significa 10%)
- `expiry_date`: Null si no caduca, sino timestamp
- `usage_limit`: Null si ilimitado, sino número máximo
- `usage_count`: Cuántas veces fue usado
- `is_active`: ¿se puede usar?
- `created_at`: Cuándo se creó

**Relaciones:**
- Independiente (referenciado en ORDERS.coupon_code_used)

---

## SQL Scripts Completos

### Crear todas las tablas

```sql
-- Users
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(MAX) NOT NULL,
    full_name VARCHAR(255),
    is_admin BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_users_email ON users(email);

-- Products
CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    base_price DECIMAL(10, 2) NOT NULL,
    category VARCHAR(100),
    image_url VARCHAR(MAX),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_products_category ON products(category);

-- Orders
CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    total_price DECIMAL(10, 2) NOT NULL,
    discount_applied DECIMAL(10, 2) DEFAULT 0,
    final_price DECIMAL(10, 2) NOT NULL,
    status VARCHAR(50) DEFAULT 'Pending',
    payment_id VARCHAR(255),
    coupon_code_used VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_orders_user ON orders(user_id);
CREATE INDEX idx_orders_status ON orders(status);

-- Order Items
CREATE TABLE order_items (
    id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    product_id INT NOT NULL REFERENCES products(id),
    quantity INT NOT NULL DEFAULT 1,
    unit_price DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_order_items_order ON order_items(order_id);

-- Designs
CREATE TABLE designs (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    image_url VARCHAR(MAX) NOT NULL,
    prompt_used TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_designs_user ON designs(user_id);

-- Coupons
CREATE TABLE coupons (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    percentage DECIMAL(5, 2) NOT NULL,
    expiry_date TIMESTAMP,
    usage_limit INT,
    usage_count INT DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_coupons_code ON coupons(code);
```

---

## Seed Data (Datos de Prueba)

```sql
-- Usuarios
INSERT INTO users (email, password_hash, full_name, is_admin) VALUES
('admin@example.com', 'HASH_SHA256', 'Admin User', TRUE),
('user1@example.com', 'HASH_SHA256', 'John Doe', FALSE),
('user2@example.com', 'HASH_SHA256', 'Jane Smith', FALSE);

-- Productos
INSERT INTO products (name, description, base_price, category) VALUES
('Remera Básica', 'Remera 100% algodón', 15.99, 'Clothing'),
('Taza Personalizada', 'Taza cerámica 350ml', 9.99, 'Mug'),
('Buzo con Capucha', 'Buzo cómodo', 29.99, 'Clothing');

-- Cupones
INSERT INTO coupons (code, percentage, is_active) VALUES
('PROMO10', 10, TRUE),
('SUMMER20', 20, TRUE),
('WELCOME5', 5, TRUE);
```

---

## Validaciones a Nivel de BD

```sql
-- Email debe ser válido (simple check)
ALTER TABLE users ADD CONSTRAINT chk_email_format 
CHECK (email LIKE '%@%');

-- Precios no negativos
ALTER TABLE products ADD CONSTRAINT chk_product_price 
CHECK (base_price >= 0);

ALTER TABLE orders ADD CONSTRAINT chk_order_price 
CHECK (final_price >= 0);

ALTER TABLE order_items ADD CONSTRAINT chk_item_quantity 
CHECK (quantity > 0);

ALTER TABLE order_items ADD CONSTRAINT chk_item_price 
CHECK (unit_price >= 0);

-- Porcentaje de cupón entre 0 y 100
ALTER TABLE coupons ADD CONSTRAINT chk_coupon_percentage 
CHECK (percentage >= 0 AND percentage <= 100);

-- Status válidos
ALTER TABLE orders ADD CONSTRAINT chk_order_status 
CHECK (status IN ('Pending', 'Paid', 'Shipped', 'Delivered', 'Cancelled'));
```

---

## Normalización

**Está normalizada en 3NF:**
- ✅ Sin dependencias funcionales de no-clave
- ✅ Sin dependencias transitivas
- ✅ Claves foráneas correctas
- ✅ Sin redundancia innecesaria

**EXCEPTO:**
- `order_items.unit_price` duplica datos de `products.base_price`
  - ✅ Es correcto: registra el precio histórico
  - De otra forma, si cambias el precio del producto, el historial sería incorrecto

---

## Escalabilidad Futura

**Si quieres agregar después (NO AHORA):**

```sql
-- Tracking de órdenes
ALTER TABLE orders ADD COLUMN tracking_number VARCHAR(100);

-- Historial de cambios
CREATE TABLE order_status_history (
    id SERIAL PRIMARY KEY,
    order_id INT REFERENCES orders(id),
    old_status VARCHAR(50),
    new_status VARCHAR(50),
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Reviews de productos
CREATE TABLE product_reviews (
    id SERIAL PRIMARY KEY,
    product_id INT REFERENCES products(id),
    user_id INT REFERENCES users(id),
    rating INT CHECK (rating >= 1 AND rating <= 5),
    comment TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Stock/inventario
ALTER TABLE products ADD COLUMN stock_quantity INT DEFAULT 0;
```

---

## Resumen

**6 Tablas:**
1. users - Usuarios
2. products - Catálogo
3. orders - Pedidos
4. order_items - Detalles de pedidos
5. designs - Galería IA
6. coupons - Descuentos

**Totales:**
- 27 campos
- 8 índices
- 5 claves foráneas
- Perfecta para MVP

---

*Diseño de BD - Mayo 2026*
