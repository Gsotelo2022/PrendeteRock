# FASE 3: Integración Frontend-Backend (Semanas 3-4)

## 🎯 Objetivo de esta Fase

Conectar tu Vue.js con el backend .NET.

**Salida:** 
- Haces login en Vue.js
- Se envía a .NET
- .NET autentica y retorna respuesta
- Vue.js muestra datos

---

## 📋 Tareas de FASE 3

### Tarea 3.1: Actualizar composables de Vue.js

**¿Qué es?** Cambiar las URLs de las peticiones HTTP.

**Dónde?** En tu proyecto Vue.js, archivo `/src/composables/useApi.js`

#### Nuevo useApi.js:

```javascript
import { ref } from 'vue'

export function useApi() {
  // IMPORTANTE: Cambiar de FastAPI (8000) a .NET (5000)
  const baseURL = 'http://localhost:5000/api'
  const token = ref(localStorage.getItem('token') || '')

  // GET request
  const get = async (endpoint) => {
    try {
      const response = await fetch(`${baseURL}${endpoint}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': token.value ? `Bearer ${token.value}` : ''
        }
      })
      
      if (!response.ok) {
        throw new Error(`Error ${response.status}`)
      }
      
      return await response.json()
    } catch (error) {
      console.error('GET Error:', error)
      throw error
    }
  }

  // POST request
  const post = async (endpoint, data) => {
    try {
      const response = await fetch(`${baseURL}${endpoint}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': token.value ? `Bearer ${token.value}` : ''
        },
        body: JSON.stringify(data)
      })
      
      if (!response.ok) {
        throw new Error(`Error ${response.status}`)
      }
      
      return await response.json()
    } catch (error) {
      console.error('POST Error:', error)
      throw error
    }
  }

  // Guardar token
  const setToken = (newToken) => {
    token.value = newToken
    localStorage.setItem('token', newToken)
  }

  // Limpiar token
  const clearToken = () => {
    token.value = ''
    localStorage.removeItem('token')
  }

  return {
    baseURL,
    token,
    get,
    post,
    setToken,
    clearToken
  }
}
```

---

### Tarea 3.2: Actualizar componentes Vue.js

**¿Qué es?** Cambiar cómo los componentes llaman al backend.

**Cambios en Login.vue:**

```javascript
<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useApi } from '@/composables/useApi'

const router = useRouter()
const { post, setToken } = useApi()

const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  
  try {
    // Enviar a .NET backend
    const response = await post('/auth/login', {
      email: email.value,
      password: password.value
    })
    
    // Guardar token
    setToken(response.token)
    
    // Ir a home
    router.push('/home')
  } catch (err) {
    error.value = 'Email o contraseña inválidos'
  } finally {
    loading.value = false
  }
}
</script>
```

**Cambios en ProductsList.vue:**

```javascript
<script setup>
import { ref, onMounted } from 'vue'
import { useApi } from '@/composables/useApi'

const { get } = useApi()
const products = ref([])
const loading = ref(true)

onMounted(async () => {
  try {
    // Obtener productos del backend
    const response = await get('/products')
    products.value = response
  } catch (error) {
    console.error('Error al cargar productos:', error)
  } finally {
    loading.value = false
  }
})
</script>
```

**Cambios en CreateOrder.vue:**

```javascript
<script setup>
import { ref } from 'vue'
import { useApi } from '@/composables/useApi'

const { post } = useApi()
const userId = ref(1) // Obtener del usuario logueado
const productId = ref('')
const quantity = ref(1)
const loading = ref(false)

const handleCreateOrder = async () => {
  loading.value = true
  
  try {
    // Crear orden en backend
    const response = await post('/orders', {
      userId: userId.value,
      productId: parseInt(productId.value),
      quantity: parseInt(quantity.value)
    })
    
    alert('Orden creada exitosamente!')
    // Ir a mis órdenes
  } catch (error) {
    alert('Error al crear orden: ' + error.message)
  } finally {
    loading.value = false
  }
}
</script>
```

---

### Tarea 3.3: Configurar CORS en Backend

**¿Qué es?** Permitir que Vue.js (localhost:5173) acceda a .NET (localhost:5000).

**Ya está configurado en Program.cs (FASE 1):**

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()
    );
});
```

---

### Tarea 3.4: Testing Completo

**Prueba 1: Login**
1. Abre http://localhost:5173 (Vue.js)
2. Ve a Sign In
3. Ingresa email: `user1@example.com` (uno que creaste en BD)
4. Ingresa password: cualquiera
5. Debería loguearte

**Prueba 2: Ver Productos**
1. Ve a Home o Create Order
2. Debería mostrar productos desde .NET

**Prueba 3: Crear Orden**
1. Selecciona un producto
2. Cantidad: 5
3. Click "Comprar"
4. Debería crear orden en .NET

---

## ✅ Checklist FASE 3

```
□ useApi.js actualizado (cambiar a localhost:5000)
□ Login.vue usa /api/auth/login
□ ProductsList.vue usa /api/products
□ CreateOrder.vue usa /api/orders
□ CORS configurado en backend
□ Vue.js ejecutándose: npm run dev (puerto 5173)
□ Backend ejecutándose: dotnet run (puerto 5000)
□ Probar login: Funciona ✓
□ Probar ver productos: Funciona ✓
□ Probar crear orden: Funciona ✓
```

**Cuando TODO esté ✓, pasas a FASE 4.**

---

## 🔧 Troubleshooting Común

### Error: "CORS error" o "Blocked by CORS"
**Solución:** CORS ya está en Program.cs (FASE 1). Si aún falla:
```csharp
// En Program.cs, ANTES de app.UseCors()
app.UseCors();  // Debe estar ANTES de .UseRouting()
```

### Error: "Cannot POST /api/orders"
**Solución:** El endpoint no existe. Verifica que:
- OrdersController esté en /Controllers/
- Esté compilado: `dotnet build`
- Backend esté ejecutándose: `dotnet run`

### Error: "TypeError: response.json() is null"
**Solución:** El backend no retorna JSON. Revisa que:
- El método retorne `Ok(data)` no `Ok()`
- Los datos estén en formato válido

---

*FASE 3 - Integración Frontend-Backend - Guía Completa para Principiantes*
