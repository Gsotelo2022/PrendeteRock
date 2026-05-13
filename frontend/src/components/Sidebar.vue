<template>
  <div class="sidebar-container">
    <!-- Sidebar - Solo mostrar si está autenticado -->
    <aside v-if="authState" class="sidebar" :class="{ 'sidebar-collapsed': isCollapsed }">
      <!-- Header -->
      <div class="sidebar-header">
        <div class="logo-container">
          <div class="logo-icon">
            <span>🎨</span>
          </div>
          <div v-if="!isCollapsed" class="logo-text">
            <h2 class="logo-title">PrendeteRock</h2>            
          </div>
        </div>
      </div>

      <!-- Navigation -->
      <nav class="sidebar-nav">
        <!-- Admin Section (solo si es admin) -->
        <div v-if="userIsAdmin" class="nav-section">
          <h3 v-if="!isCollapsed" class="nav-section-title">⚙️ Administración</h3>

          <router-link to="/admin" class="nav-item" :title="isCollapsed ? 'Dashboard' : ''">
            <span class="nav-icon">📊</span>
            <span v-if="!isCollapsed" class="nav-text">Dashboard</span>
          </router-link>

          <router-link to="/admin/clientes" class="nav-item" :title="isCollapsed ? 'Customers' : ''">
            <span class="nav-icon">👥</span>
            <span v-if="!isCollapsed" class="nav-text">Clientes</span>
          </router-link>

          <router-link to="/admin/productos" class="nav-item" :title="isCollapsed ? 'Products' : ''">
            <span class="nav-icon">📦</span>
            <span v-if="!isCollapsed" class="nav-text">Productos</span>
          </router-link>

          <router-link to="/admin/cupones" class="nav-item" :title="isCollapsed ? 'Coupons' : ''">
            <span class="nav-icon">🎟️</span>
            <span v-if="!isCollapsed" class="nav-text">Cupones</span>
          </router-link>
        </div>

        <!-- Customer Section (solo si no es admin) -->
        <div v-if="!userIsAdmin" class="nav-section">
          <h3 v-if="!isCollapsed" class="nav-section-title">🎨 Cliente</h3>

          <router-link to="/home" class="nav-item" :title="isCollapsed ? 'Home' : ''">
            <span class="nav-icon">🏠</span>
            <span v-if="!isCollapsed" class="nav-text">Inicio</span>
          </router-link>

          <router-link to="/cliente/crear-orden" class="nav-item" :title="isCollapsed ? 'Create Order' : ''">
            <span class="nav-icon">➕</span>
            <span v-if="!isCollapsed" class="nav-text">Crear Orden</span>
          </router-link>

          <router-link to="/cliente/mis-ordenes" class="nav-item" :title="isCollapsed ? 'My Orders' : ''">
            <span class="nav-icon">🛒</span>
            <span v-if="!isCollapsed" class="nav-text">Mis Órdenes</span>
          </router-link>

          <router-link to="/cliente/mis-diseños" class="nav-item" :title="isCollapsed ? 'My Designs' : ''">
            <span class="nav-icon">🖼️</span>
            <span v-if="!isCollapsed" class="nav-text">Mis Diseños</span>
          </router-link>
        </div>
      </nav>

      <!-- Footer -->
      <div class="sidebar-footer">
        <div class="footer-buttons">
          <button class="sign-in-btn" @click="handleLogout" :title="isCollapsed ? 'Sign Out' : ''">
            <span v-if="!isCollapsed">Cerrar Sesión</span>
            <span v-else>↪️</span>
          </button>
          <button class="toggle-btn" @click="toggleSidebar" :title="isCollapsed ? 'Expandir' : 'Contraer'">
            <span>{{ isCollapsed ? '▶' : '◀' }}</span>
          </button>
        </div>
      </div>
    </aside>

    <!-- Content Area -->
    <div class="content-wrapper" :class="{ 'content-full': !authState, 'content-collapsed': authState && isCollapsed }">
      <slot></slot>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, onUpdated, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useApi } from '@/composables/useApi'

const router = useRouter()
const { clearToken } = useApi()

const isCollapsed = ref(false)
const userIsAdmin = ref(false)
const authState = ref(false)
const updateTrigger = ref(0)

// Check auth status by reading token from localStorage directly
const checkAuthStatus = () => {
  const tokenFromStorage = localStorage.getItem('auth_token')
  const isAuth = !!tokenFromStorage
  
  authState.value = isAuth
  
  if (isAuth && tokenFromStorage) {
    try {
      // Decodificar JWT
      const parts = tokenFromStorage.split('.')
      if (parts.length === 3) {
        const decoded = JSON.parse(atob(parts[1]))
        // Verificar isadmin - el backend lo devuelve como string "true" o "false"
        userIsAdmin.value = decoded.isadmin === 'true' || decoded.isadmin === true
      } else {
        userIsAdmin.value = false
      }
    } catch (error) {
      console.error('Error decoding token:', error)
      userIsAdmin.value = false
    }
  } else {
    userIsAdmin.value = false
  }
}

const toggleSidebar = () => {
  isCollapsed.value = !isCollapsed.value
  localStorage.setItem('sidebarCollapsed', isCollapsed.value)
}

const handleLogout = () => {
  clearToken()
  userIsAdmin.value = false
  authState.value = false
  // Force immediate update
  setTimeout(() => {
    checkAuthStatus()
    router.push('/login')
  }, 50)
}

// Listen for storage changes (when logout happens)
const handleStorageChange = () => {
  checkAuthStatus()
}

// Listen for auth changes
const handleAuthChange = () => {
  checkAuthStatus()
}

// Update auth status whenever component updates
onUpdated(() => {
  checkAuthStatus()
})

onMounted(() => {
  const saved = localStorage.getItem('sidebarCollapsed')
  if (saved !== null) {
    isCollapsed.value = JSON.parse(saved)
  }
  
  // Initial check on mount
  checkAuthStatus()
  
  // Listen for storage changes from other tabs/windows
  window.addEventListener('storage', handleStorageChange)
  
  // Listen for custom auth-changed event from useApi
  window.addEventListener('auth-changed', handleAuthChange)
  
  // Check auth status periodically (every 300ms) to catch token changes
  const interval = setInterval(() => {
    checkAuthStatus()
  }, 300)
  
  // Cleanup on unmount
  onBeforeUnmount(() => {
    clearInterval(interval)
    window.removeEventListener('storage', handleStorageChange)
    window.removeEventListener('auth-changed', handleAuthChange)
  })
})
</script>

<style scoped>
.sidebar-container {
  display: flex;
  min-height: 100vh;
  width: 100%;
  position: relative;
}

/* Sidebar */
.sidebar {
  width: 280px;
  background: linear-gradient(135deg, #faf7f2 0%, #f5f1e8 100%);
  border-right: 2px solid #e0d5c8;
  display: flex;
  flex-direction: column;
  transition: all 0.3s ease;
  position: fixed;
  left: 0;
  top: 0;
  height: 100vh;
  flex-shrink: 0;
  z-index: 100;
  box-shadow: 2px 0 10px rgba(0, 0, 0, 0.05);
}

.sidebar.sidebar-collapsed {
  width: 80px;
}

/* Sidebar Header */
.sidebar-header {
  padding: 25px 20px;
  border-bottom: 2px solid #e0d5c8;
  background: linear-gradient(135deg, rgba(160, 32, 240, 0.05) 0%, rgba(255, 0, 110, 0.02) 100%);
}

.logo-container {
  display: flex;
  align-items: center;
  gap: 15px;
}

.logo-icon {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  flex-shrink: 0;
  box-shadow: 0 4px 12px rgba(160, 32, 240, 0.25);
  transition: all 0.3s ease;
}

.logo-icon:hover {
  transform: scale(1.05);
}

.logo-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.logo-title {
  font-size: 1.1rem;
  font-weight: 800;
  margin: 0;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.logo-subtitle {
  font-size: 0.75rem;
  color: #999;
  margin: 0;
  font-weight: 500;
}

/* Toggle Button */
.toggle-btn {
  display: flex;
  position: relative;
  width: 40px;
  height: 40px;
  border-radius: 8px;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  border: none;
  cursor: pointer;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  color: white;
  transition: all 0.3s ease;
  font-weight: 700;
  padding: 0;
  flex-shrink: 0;
}

.toggle-btn:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(160, 32, 240, 0.35);
}

.toggle-btn:active {
  transform: scale(0.95);
}

/* Navigation */
.sidebar-nav {
  flex: 1;
  overflow-y: auto;
  padding: 20px 0;
}

.nav-section {
  margin-bottom: 10px;
  padding: 15px 10px;
  border-radius: 12px;
}

.nav-section:first-child {
  margin-top: 10px;
}

.nav-section-title {
  font-size: 0.7rem;
  color: #a020f0;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  font-weight: 800;
  margin: 0 0 12px 5px;
  padding: 8px 0;
  border-bottom: 2px solid #a020f0;
  display: inline-block;
}

.nav-item {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 11px 12px;
  background: none;
  border: none;
  color: #555;
  font-size: 0.93rem;
  font-weight: 500;
  cursor: pointer;
  border-radius: 8px;
  transition: all 0.25s ease;
  margin-bottom: 6px;
  text-align: left;
  justify-content: flex-start;
  text-decoration: none;
  border-left: 3px solid transparent;
}

.nav-item:hover {
  background: #f0e6ff;
  color: #a020f0;
  border-left-color: #a020f0;
  padding-left: 15px;
  box-shadow: 0 2px 8px rgba(160, 32, 240, 0.08);
}

.nav-item.router-link-active {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  color: white;
  border-left-color: white;
  font-weight: 600;
  box-shadow: 0 4px 12px rgba(160, 32, 240, 0.25);
}

.nav-item.router-link-active:hover {
  background: linear-gradient(135deg, #8a1adf 0%, #e60060 100%);
  box-shadow: 0 6px 16px rgba(160, 32, 240, 0.35);
}

.nav-icon {
  font-size: 1.4rem;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
}

.nav-item.router-link-active .nav-icon {
  filter: drop-shadow(0 2px 4px rgba(255, 255, 255, 0.3));
}

.nav-text {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  flex: 1;
}

/* Sidebar Footer */
.sidebar-footer {
  padding: 15px 15px;
  border-top: 2px solid #e0d5c8;
  background: rgba(160, 32, 240, 0.03);
}

.footer-buttons {
  display: flex;
  gap: 10px;
  align-items: center;
}

.sign-in-btn {
  flex: 1;
  padding: 12px 15px;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 0.93rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.3s ease;
  text-decoration: none;
  display: flex;
  align-items: center;
  justify-content: center;
}

.sign-in-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(160, 32, 240, 0.35);
  background: linear-gradient(135deg, #8a1adf 0%, #e60060 100%);
}

.sign-in-btn:active {
  transform: translateY(0);
}

/* Content Wrapper */
.content-wrapper {
  flex: 1;
  overflow-y: auto;
  transition: all 0.3s ease;
  width: 100%;
  margin-left: 280px;
  background: #fff;
}

.content-wrapper.content-full {
  margin-left: 0;
  width: 100%;
}

.content-wrapper.content-collapsed {
  margin-left: 80px;
}

/* Scrollbar Styling */
.sidebar-nav::-webkit-scrollbar {
  width: 6px;
}

.sidebar-nav::-webkit-scrollbar-track {
  background: transparent;
}

.sidebar-nav::-webkit-scrollbar-thumb {
  background: rgba(160, 32, 240, 0.3);
  border-radius: 3px;
}

.sidebar-nav::-webkit-scrollbar-thumb:hover {
  background: rgba(160, 32, 240, 0.5);
}

/* Responsive */
@media (max-width: 768px) {
  .sidebar {
    position: fixed;
    left: 0;
    top: 0;
    height: 100vh;
    z-index: 999;
    box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
  }

  .sidebar.sidebar-collapsed {
    transform: translateX(-100%);
  }

  .content-wrapper {
    width: 100%;
    margin-left: 0;
  }

  .content-wrapper.content-collapsed {
    margin-left: 0;
  }

  .toggle-btn {
    position: fixed;
    top: 20px;
    right: 20px;
    width: 50px;
    height: 50px;
    border-radius: 8px;
    background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
    border: none;
    z-index: 1001;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    font-size: 1.5rem;
    color: white;
    padding: 0;
  }

  .toggle-btn:hover {
    transform: scale(1.08);
    box-shadow: 0 6px 16px rgba(160, 32, 240, 0.3);
  }
}

@media (max-width: 480px) {
  .sidebar {
    width: 250px;
  }

  .nav-item {
    padding: 10px 12px;
    font-size: 0.85rem;
  }

  .nav-icon {
    font-size: 1.1rem;
  }

  .logo-icon {
    width: 45px;
    height: 45px;
    font-size: 1.2rem;
  }

  .logo-title {
    font-size: 1rem;
  }

  .logo-subtitle {
    font-size: 0.7rem;
  }
}
</style>
