<template>
  <section class="hero">
    <div class="hero-content">
      <h1 class="hero-title">Transforma Tus Ideas en Hermosas Impresiones</h1>
      <p class="hero-description">Crea impresiones personalizadas con IA o sube tus propios diseños. Impresión de alta calidad en materiales premium entregados a tu puerta.</p>
      <div class="hero-buttons">
        <button class="btn btn-primary" @click="handlePrimaryButton">
          <span class="btn-icon">✨</span> {{ primaryButtonText }}
        </button>
        <button class="btn btn-secondary" @click="handleSecondaryButton">
          <span class="btn-icon">🖼️</span> {{ secondaryButtonText }}
        </button>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useApi } from '@/composables/useApi'

const router = useRouter()
const { isAuthenticated } = useApi()

const isLoggedIn = ref(false)
const userIsAdmin = ref(false)

const primaryButtonText = computed(() => {
  if (isLoggedIn.value && !userIsAdmin.value) {
    return 'Crear Orden'
  }
  return 'Comenzar Ahora'
})

const secondaryButtonText = computed(() => {
  if (isLoggedIn.value && !userIsAdmin.value) {
    return 'Mis Órdenes'
  }
  return 'Registrarme'
})

const updateAuthStatus = () => {
  const tokenFromStorage = localStorage.getItem('auth_token')
  isLoggedIn.value = !!tokenFromStorage
  
  if (isLoggedIn.value && tokenFromStorage) {
    try {
      const parts = tokenFromStorage.split('.')
      if (parts.length === 3) {
        const decoded = JSON.parse(atob(parts[1]))
        userIsAdmin.value = decoded.isadmin === 'true' || decoded.isadmin === true
      }
    } catch (error) {
      console.error('Error decoding token:', error)
      userIsAdmin.value = false
    }
  }
}

const handlePrimaryButton = () => {
  if (isLoggedIn.value && !userIsAdmin.value) {
    router.push('/cliente/crear-orden')
  } else {
    router.push('/login')
  }
}

const handleSecondaryButton = () => {
  if (isLoggedIn.value && !userIsAdmin.value) {
    router.push('/cliente/mis-ordenes')
  } else {
    router.push('/register')
  }
}

onMounted(() => {
  updateAuthStatus()
  
  // Listen for auth changes
  window.addEventListener('auth-changed', updateAuthStatus)
  
  // Monitor localStorage changes
  const interval = setInterval(updateAuthStatus, 300)
  
  return () => {
    clearInterval(interval)
    window.removeEventListener('auth-changed', updateAuthStatus)
  }
})
</script>

<style scoped>
.hero {
  background: linear-gradient(135deg, #f5f1e8 0%, #f5f1e8 100%);
  padding: 80px 20px;
  text-align: center;
  min-height: 500px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.hero-content {
  max-width: 800px;
  margin: 0 auto;
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 900;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin-bottom: 20px;
  line-height: 1.2;
}

.hero-description {
  font-size: 1.2rem;
  color: #5a5a5a;
  margin-bottom: 40px;
  line-height: 1.6;
}

.hero-buttons {
  display: flex;
  gap: 20px;
  justify-content: center;
  flex-wrap: wrap;
}

.btn {
  padding: 15px 40px;
  font-size: 1rem;
  border: none;
  border-radius: 50px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-weight: 600;
}

.btn-primary {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  color: white;
  display: flex;
  align-items: center;
  gap: 10px;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(160, 32, 240, 0.3);
}

.btn-secondary {
  background-color: white;
  color: #333;
  border: 2px solid #ddd;
}

.btn-secondary:hover {
  border-color: #a020f0;
  color: #a020f0;
  transform: translateY(-2px);
}

.btn-icon {
  font-size: 1.2rem;
}

@media (max-width: 768px) {
  .hero-title {
    font-size: 2.5rem;
  }

  .hero-description {
    font-size: 1rem;
  }

  .hero-buttons {
    flex-direction: column;
    gap: 15px;
  }

  .btn {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .hero {
    padding: 40px 20px;
    min-height: auto;
  }

  .hero-title {
    font-size: 2rem;
  }
}
</style>
