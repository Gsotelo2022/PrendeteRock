<template>
  <div id="app">
    <!-- Register Section -->
    <section class="register-section">
      <div class="register-container">
        <button class="back-btn" @click="goHome">
          <span>←</span> Volver
        </button>
        
        <div class="register-content">
          <h1 class="register-title">Crea tu Cuenta</h1>
          <p class="register-subtitle">Únete a PrintHub y comienza a crear</p>

          <form @submit.prevent="handleRegister" class="register-form">
            <div v-if="error" class="error-message">{{ error }}</div>
            
            <div class="form-row">
              <div class="form-group">
                <label for="firstName">Nombre</label>
                <input 
                  type="text" 
                  id="firstName" 
                  placeholder="Juan"
                  v-model="firstName"
                  :disabled="loading"
                  required
                >
              </div>

              <div class="form-group">
                <label for="lastName">Apellido</label>
                <input 
                  type="text" 
                  id="lastName" 
                  placeholder="Pérez"
                  v-model="lastName"
                  :disabled="loading"
                  required
                >
              </div>
            </div>

            <div class="form-group">
              <label for="email">Correo Electrónico</label>
              <input 
                type="email" 
                id="email" 
                placeholder="tu@email.com"
                v-model="email"
                :disabled="loading"
                required
              >
            </div>

            <div class="form-group">
              <label for="password">Contraseña</label>
              <input 
                type="password" 
                id="password" 
                placeholder="Mínimo 8 caracteres"
                v-model="password"
                :disabled="loading"
                required
                minlength="8"
              >
              <small class="password-hint">Mínimo 8 caracteres con números y letras</small>
            </div>

            <div class="form-group">
              <label for="confirmPassword">Confirmar Contraseña</label>
              <input 
                type="password" 
                id="confirmPassword" 
                placeholder="••••••••"
                v-model="confirmPassword"
                :disabled="loading"
                required
              >
            </div>

            <div class="form-group checkbox">
              <input 
                type="checkbox" 
                id="terms" 
                v-model="agreeTerms"
                :disabled="loading"
                required
              >
              <label for="terms">
                Acepto los 
                <button type="button" @click="handleTerms" class="link-btn" :disabled="loading">términos y condiciones</button>
              </label>
            </div>

            <button type="submit" class="btn btn-submit" :disabled="loading">
              {{ loading ? 'Registrando...' : 'Crear Cuenta' }}
            </button>
          </form>

          <div class="divider">O</div>

          <button class="btn btn-google">
            <span>🔍</span> Registrarse con Google
          </button>

          <div class="login-link">
            ¿Ya tienes cuenta? 
            <button @click="goLogin" class="link-btn">Inicia sesión</button>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useApi } from '@/composables/useApi'

const router = useRouter()
const { post, setToken } = useApi()

const firstName = ref('')
const lastName = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const agreeTerms = ref(false)
const error = ref('')
const loading = ref(false)

const handleRegister = async () => {
  if (password.value !== confirmPassword.value) {
    error.value = 'Las contraseñas no coinciden'
    return
  }

  if (!agreeTerms.value) {
    error.value = 'Debes aceptar los términos y condiciones'
    return
  }

  loading.value = true
  error.value = ''
  
  try {
    // Enviar a .NET backend
    const response = await post('/auth/register', {
      email: email.value,
      password: password.value,
      fullName: `${firstName.value} ${lastName.value}`
    })
    
    // Guardar token
    setToken(response.token)
    
    // Decodificar token para verificar si es admin
    let destination = '/home'
    try {
      const parts = response.token.split('.')
      if (parts.length === 3) {
        const decoded = JSON.parse(atob(parts[1]))
        if (decoded.isadmin === 'true' || decoded.isadmin === true) {
          destination = '/admin'
        }
      }
    } catch (decodeError) {
      console.error('Error decoding token:', decodeError)
    }
    
    // Limpiar formulario
    firstName.value = ''
    lastName.value = ''
    email.value = ''
    password.value = ''
    confirmPassword.value = ''
    agreeTerms.value = false
    
    // Ir a destino correcto
    router.push(destination)
  } catch (err) {
    console.error('Register error:', err)
    error.value = 'Error al registrarse. Intenta de nuevo'
  } finally {
    loading.value = false
  }
}

const handleTerms = () => {
  alert('Términos y condiciones en desarrollo')
}

const goHome = () => {
  router.push('/')
}

const goLogin = () => {
  router.push('/login')
}
</script>

<style scoped>
#app {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background-color: #faf7f2;
  min-height: 100vh;
}

.register-section {
  padding: 40px 20px;
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
}

.register-container {
  width: 100%;
  max-width: 500px;
}

.back-btn {
  background: none;
  border: none;
  color: #666;
  font-size: 1rem;
  cursor: pointer;
  padding: 10px;
  display: flex;
  align-items: center;
  gap: 5px;
  margin-bottom: 30px;
  transition: color 0.3s ease;
}

.back-btn:hover {
  color: #a020f0;
}

.register-content {
  background: white;
  padding: 50px 40px;
  border-radius: 20px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.08);
}

.register-title {
  font-size: 2rem;
  font-weight: 800;
  color: #1a1a1a;
  margin-bottom: 10px;
}

.register-subtitle {
  color: #666;
  font-size: 1rem;
  margin-bottom: 30px;
}

.register-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-bottom: 30px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 15px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 600;
  color: #1a1a1a;
  font-size: 0.95rem;
}

.form-group input {
  padding: 12px 15px;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  font-size: 1rem;
  transition: all 0.3s ease;
  font-family: inherit;
}

.form-group input:focus {
  outline: none;
  border-color: #a020f0;
  box-shadow: 0 0 0 3px rgba(160, 32, 240, 0.1);
}

.form-group input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
  opacity: 0.6;
}

.error-message {
  padding: 12px 15px;
  background-color: #fee;
  border: 1px solid #fcc;
  border-radius: 8px;
  color: #c33;
  font-size: 0.95rem;
  margin-bottom: 15px;
}

.password-hint {
  font-size: 0.85rem;
  color: #999;
  line-height: 1.3;
}

.form-group.checkbox {
  flex-direction: row;
  align-items: flex-start;
  gap: 10px;
  margin-top: 5px;
}

.form-group.checkbox input[type="checkbox"] {
  width: 20px;
  height: 20px;
  margin-top: 3px;
  cursor: pointer;
  accent-color: #a020f0;
}

.form-group.checkbox label {
  font-weight: 500;
  margin: 0;
  font-size: 0.95rem;
}

.btn {
  padding: 12px 20px;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
  font-family: inherit;
}

.btn-submit {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  color: white;
  padding: 13px 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-submit:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(160, 32, 240, 0.3);
}

.btn-submit:active:not(:disabled) {
  transform: translateY(0);
}

.btn-submit:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.divider {
  text-align: center;
  color: #999;
  margin: 25px 0;
  position: relative;
}

.divider::before,
.divider::after {
  content: '';
  position: absolute;
  top: 50%;
  width: 45%;
  height: 1px;
  background-color: #e0e0e0;
}

.divider::before {
  left: 0;
}

.divider::after {
  right: 0;
}

.btn-google {
  background-color: white;
  color: #333;
  border: 2px solid #e0e0e0;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  width: 100%;
}

.btn-google:hover {
  border-color: #a020f0;
  color: #a020f0;
  transform: translateY(-2px);
}

.login-link {
  text-align: center;
  color: #666;
  margin-top: 25px;
  font-size: 0.95rem;
}

.link-btn {
  background: none;
  border: none;
  color: #a020f0;
  font-weight: 600;
  cursor: pointer;
  text-decoration: none;
  padding: 0;
  font-size: inherit;
  transition: color 0.3s ease;
}

.link-btn:hover {
  color: #ff006e;
  text-decoration: underline;
}

@media (max-width: 600px) {
  .form-row {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 480px) {
  .register-content {
    padding: 35px 25px;
  }

  .register-title {
    font-size: 1.5rem;
  }

  .register-subtitle {
    font-size: 0.9rem;
  }
}
</style>
