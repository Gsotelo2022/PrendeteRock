<template>
  <section class="cta-section">
    <h2>¿Listo para Crear?</h2>
    <p>Únete a cientos de clientes felices que confían en PrendeteRock para sus impresiones personalizadas</p>
    <button class="btn btn-cta" @click="handleCTA">{{ buttonText }}</button>
  </section>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const isLoggedIn = ref(false)
const userIsAdmin = ref(false)

const buttonText = computed(() => {
  if (isLoggedIn.value && !userIsAdmin.value) {
    return 'Crear Orden Ahora'
  }
  return 'Comenzar Ahora'
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

const handleCTA = () => {
  if (isLoggedIn.value && !userIsAdmin.value) {
    router.push('/cliente/crear-orden')
  } else {
    router.push('/login')
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
.cta-section {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  padding: 80px 20px;
  text-align: center;
  color: white;
  margin: 80px 20px;
  border-radius: 20px;
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
}

.cta-section h2 {
  font-size: 2.5rem;
  font-weight: 800;
  margin-bottom: 20px;
}

.cta-section p {
  font-size: 1.1rem;
  margin-bottom: 40px;
  opacity: 0.95;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

.btn {
  padding: 15px 40px;
  border: none;
  border-radius: 50px;
  cursor: pointer;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
  font-family: inherit;
}

.btn-cta {
  background: white;
  color: #a020f0;
  padding: 15px 50px;
  font-size: 1.1rem;
}

.btn-cta:hover {
  transform: translateY(-2px);
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.2);
}

@media (max-width: 768px) {
  .cta-section h2 {
    font-size: 2rem;
  }
}
</style>
