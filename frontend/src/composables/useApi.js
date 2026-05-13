import { ref } from 'vue'

// Crear estado compartido global
let globalToken = localStorage.getItem('auth_token') || ''

export function useApi() {
  // IMPORTANTE: Backend .NET en puerto 5000
  const baseURL = 'http://localhost:5000/api'
  const token = ref(globalToken)

  // Headers helper
  const getHeaders = () => ({
    'Content-Type': 'application/json',
    ...(token.value && { 'Authorization': `Bearer ${token.value}` })
  })

  // GET request
  const get = async (endpoint) => {
    try {
      const response = await fetch(`${baseURL}${endpoint}`, {
        method: 'GET',
        headers: getHeaders()
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
        headers: getHeaders(),
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

  // PUT request
  const put = async (endpoint, data) => {
    try {
      const response = await fetch(`${baseURL}${endpoint}`, {
        method: 'PUT',
        headers: getHeaders(),
        body: JSON.stringify(data)
      })
      
      if (!response.ok) {
        throw new Error(`Error ${response.status}`)
      }
      
      return await response.json()
    } catch (error) {
      console.error('PUT Error:', error)
      throw error
    }
  }

  // DELETE request
  const del = async (endpoint) => {
    try {
      const response = await fetch(`${baseURL}${endpoint}`, {
        method: 'DELETE',
        headers: getHeaders()
      })
      
      if (!response.ok) {
        throw new Error(`Error ${response.status}`)
      }
      
      return await response.json()
    } catch (error) {
      console.error('DELETE Error:', error)
      throw error
    }
  }

  // Guardar token
  const setToken = (newToken) => {
    globalToken = newToken
    token.value = newToken
    localStorage.setItem('auth_token', newToken)
    // Trigger storage event for other components
    window.dispatchEvent(new Event('auth-changed'))
  }

  // Limpiar token
  const clearToken = () => {
    globalToken = ''
    token.value = ''
    localStorage.removeItem('auth_token')
    window.dispatchEvent(new Event('auth-changed'))
  }

  // Obtener token
  const getToken = () => token.value

  // Decodificar JWT y obtener datos del usuario
  const getUserFromToken = () => {
    if (!token.value) return null
    
    try {
      const parts = token.value.split('.')
      if (parts.length !== 3) return null
      
      const decoded = JSON.parse(atob(parts[1]))
      return decoded
    } catch (error) {
      console.error('Error decoding token:', error)
      return null
    }
  }

  // Verificar si el usuario es admin
  const isAdmin = () => {
    const user = getUserFromToken()
    return user?.isadmin === 'true' || user?.isadmin === true
  }

  // Verificar si está autenticado
  const isAuthenticated = () => !!token.value

  return {
    baseURL,
    token,
    get,
    post,
    put,
    del,
    setToken,
    clearToken,
    getToken,
    getUserFromToken,
    isAdmin,
    isAuthenticated
  }
}