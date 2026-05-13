<template>
  <div class="gestion-clientes">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="goHome">
        <span>←</span> Volver al Inicio
      </button>
      <h1 class="page-title">Gestión de Clientes</h1>
      <p class="page-subtitle">Ver y gestionar información de clientes</p>
    </div>

    <!-- Search Bar -->
    <div class="search-section">
      <input 
        v-model="searchQuery"
        type="text" 
        class="search-input"
        placeholder="Buscar por nombre o email..."
      >
    </div>

    <!-- Customers List -->
    <div class="customers-list">
      <div v-if="loading" class="loading-state">
        <p>Cargando clientes...</p>
      </div>
      
      <div v-else-if="error" class="error-state">
        <p>{{ error }}</p>
        <button @click="loadCustomers" class="retry-btn">Reintentar</button>
      </div>
      
      <div v-else-if="filteredCustomers.length === 0" class="empty-state">
        <p>No se encontraron clientes</p>
      </div>
      
      <div v-for="customer in filteredCustomers" :key="customer.id" class="customer-card">
        <div class="customer-header">
          <div class="avatar" :style="{ background: customer.avatarBg }">
            {{ customer.initials }}
          </div>
          <div class="customer-info">
            <h3 class="customer-name">{{ customer.name }}</h3>
            <p class="customer-id">{{ customer.customerId }}</p>
          </div>
        </div>

        <div class="customer-contact">
          <p class="contact-item">
            <span class="contact-label">📧 Email:</span>
            <span class="contact-value">{{ customer.email }}</span>
          </p>
          <p class="contact-item">
            <span class="contact-label">📞 Teléfono:</span>
            <span class="contact-value">{{ customer.phone }}</span>
          </p>
        </div>

        <div class="customer-stats">
          <div class="stat">
            <span class="stat-label">Órdenes Totales</span>
            <span class="stat-value">{{ customer.totalOrders }}</span>
          </div>
          <div class="stat">
            <span class="stat-label">Gastado</span>
            <span class="stat-value">${{ customer.totalSpent }}</span>
          </div>
          <div class="stat">
            <span class="stat-label">Fecha de Registro</span>
            <span class="stat-value">{{ customer.joinDate }}</span>
          </div>
        </div>

        <div class="customer-actions">
          <button class="action-btn view-btn" @click="handleViewDetails(customer.id)">Ver Detalles</button>
          <button class="action-btn edit-btn" @click="handleEdit(customer)">Editar</button>
          <button class="action-btn delete-btn" @click="handleDelete(customer.id)">Eliminar</button>
        </div>
      </div>
    </div>

    <!-- Modal de Edición -->
    <div v-if="showEditModal" class="modal-overlay" @click="closeEditModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2 class="modal-title">Editar Cliente</h2>
          <button class="modal-close" @click="closeEditModal">✕</button>
        </div>

        <form @submit.prevent="saveChanges" class="edit-form">
          <div class="form-group">
            <label for="fullName">Nombre Completo</label>
            <input 
              v-model="editForm.fullName"
              type="text" 
              id="fullName"
              class="form-control"
              placeholder="Ingrese el nombre completo"
              required
            >
          </div>

          <div class="form-group">
            <label for="email">Email</label>
            <input 
              v-model="editForm.email"
              type="email" 
              id="email"
              class="form-control"
              placeholder="Ingrese el email"
              required
            >
          </div>

          <div class="form-group">
            <label for="phone">Teléfono</label>
            <input 
              v-model="editForm.phone"
              type="tel" 
              id="phone"
              class="form-control"
              placeholder="Ingrese el teléfono (opcional)"
            >
          </div>

          <div class="form-group readonly-info">
            <label>ID del Cliente</label>
            <p class="readonly-value">{{ editForm.customerId }}</p>
          </div>

          <div class="form-group readonly-info">
            <label>Fecha de Registro</label>
            <p class="readonly-value">{{ editForm.joinDate }}</p>
          </div>

          <div v-if="editError" class="error-message">
            {{ editError }}
          </div>

          <div class="modal-actions">
            <button type="button" class="btn-cancel" @click="closeEditModal" :disabled="saving">
              Cancelar
            </button>
            <button type="submit" class="btn-save" :disabled="saving">
              {{ saving ? 'Guardando...' : 'Guardar Cambios' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import { useRouter } from 'vue-router'
import { useApi } from '../../composables/useApi'
import { useToast } from '../../composables/useToast'

export default {
  name: 'GestionClientes',
  setup() {
    const router = useRouter()
    const api = useApi()
    const toast = useToast()
    return { router, api, toast }
  },
  data() {
    return {
      searchQuery: '',
      customers: [],
      loading: true,
      error: null,
      showEditModal: false,
      editForm: {
        id: null,
        fullName: '',
        email: '',
        phone: '',
        customerId: '',
        joinDate: ''
      },
      saving: false,
      editError: null
    }
  },
  computed: {
    filteredCustomers() {
      if (!this.searchQuery) {
        return this.customers
      }
      const query = this.searchQuery.toLowerCase()
      return this.customers.filter(customer =>
        customer.name.toLowerCase().includes(query) ||
        customer.email.toLowerCase().includes(query) ||
        customer.customerId.toLowerCase().includes(query)
      )
    }
  },
  async mounted() {
    await this.loadCustomers()
  },
  methods: {
    // Cargar clientes desde la API
    async loadCustomers() {
      try {
        this.loading = true
        this.error = null
        const response = await this.api.get('/users/clients')
        
        // Transformar datos del backend al formato del frontend
        this.customers = response.map((user, index) => ({
          id: user.id,
          name: user.fullName,
          customerId: `CUST-${String(user.id).padStart(3, '0')}`,
          email: user.email,
          phone: user.phone || 'N/A',
          initials: this.getInitials(user.fullName),
          avatarBg: this.getAvatarGradient(index),
          totalOrders: user.totalOrders,
          totalSpent: user.totalSpent,
          joinDate: this.formatDate(user.createdAt)
        }))
      } catch (error) {
        console.error('Error al cargar clientes:', error)
        this.error = 'Error al cargar los clientes'
      } finally {
        this.loading = false
      }
    },

    // Obtener iniciales del nombre
    getInitials(fullName) {
      const names = fullName.trim().split(' ')
      if (names.length === 1) {
        return names[0].substring(0, 2).toUpperCase()
      }
      return (names[0][0] + names[names.length - 1][0]).toUpperCase()
    },

    // Generar gradiente del avatar
    getAvatarGradient(index) {
      const gradients = [
        'linear-gradient(135deg, #a020f0 0%, #ff006e 100%)',
        'linear-gradient(135deg, #ff006e 0%, #ff7a00 100%)',
        'linear-gradient(135deg, #ff7a00 0%, #a020f0 100%)',
        'linear-gradient(135deg, #a020f0 0%, #ff7a00 100%)',
        'linear-gradient(135deg, #ff006e 0%, #a020f0 100%)'
      ]
      return gradients[index % gradients.length]
    },

    // Formatear fecha
    formatDate(dateString) {
      const date = new Date(dateString)
      const months = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic']
      return `${months[date.getMonth()]} ${date.getFullYear()}`
    },

    goHome() {
      this.router.push('/admin')
    },

    handleViewDetails(customerId) {
      console.log('Ver detalles del cliente:', customerId)
      // TODO: Implementar vista de detalles
    },

    handleEdit(customer) {
      this.editForm = {
        id: customer.id,
        fullName: customer.name,
        email: customer.email,
        phone: customer.phone === 'N/A' ? '' : customer.phone,
        customerId: customer.customerId,
        joinDate: customer.joinDate
      }
      this.editError = null
      this.showEditModal = true
    },

    closeEditModal() {
      this.showEditModal = false
      this.editError = null
      this.editForm = {
        id: null,
        fullName: '',
        email: '',
        phone: '',
        customerId: '',
        joinDate: ''
      }
    },

    async saveChanges() {
      try {
        this.saving = true
        this.editError = null

        await this.api.put(`/users/${this.editForm.id}`, {
          fullName: this.editForm.fullName,
          email: this.editForm.email,
          phone: this.editForm.phone || null
        })

        // Recargar lista de clientes
        await this.loadCustomers()
        
        // Cerrar modal
        this.closeEditModal()
        
        this.toast.success('Cliente actualizado exitosamente')
      } catch (error) {
        console.error('Error al actualizar cliente:', error)
        this.editError = 'Error al actualizar el cliente. Por favor, intente nuevamente.'
        this.toast.error('Error al actualizar el cliente')
      } finally {
        this.saving = false
      }
    },

    async handleDelete(customerId) {
      if (confirm('¿Estás seguro de que deseas eliminar este cliente?')) {
        try {
          await this.api.del(`/users/${customerId}`)
          // Recargar lista de clientes
          await this.loadCustomers()
          this.toast.success('Cliente eliminado exitosamente')
        } catch (error) {
          console.error('Error al eliminar cliente:', error)
          this.toast.error('Error al eliminar el cliente')
        }
      }
    }
  }
}
</script>

<style scoped>
.gestion-clientes {
  padding: 40px 0;
  background-color: #faf7f2;
  min-height: 100vh;
}

/* Header */
.header {
  margin-bottom: 40px;
  padding: 0 20px;
}

.back-btn {
  background: none;
  border: none;
  color: #a020f0;
  font-size: 1rem;
  cursor: pointer;
  padding: 0;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 5px;
  font-weight: 600;
  transition: color 0.3s ease;
}

.back-btn:hover {
  color: #ff006e;
}

.page-title {
  font-size: 2.5rem;
  font-weight: 800;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0 0 10px 0;
}

.page-subtitle {
  font-size: 1rem;
  color: #666;
  margin: 0;
}

/* Search Section */
.search-section {
  margin-bottom: 30px;
}

.search-input {
  width: 100%;
  max-width: 500px;
  padding: 15px 20px;
  font-size: 1rem;
  border: 2px solid #e0e0e0;
  border-radius: 15px;
  background: white;
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #a020f0;
  box-shadow: 0 0 0 3px rgba(160, 32, 240, 0.1);
}

.search-input::placeholder {
  color: #999;
}

/* Customers List */
.customers-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(450px, 1fr));
  gap: 25px;
  padding: 0 20px;
}

.customer-card {
  background: white;
  padding: 25px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.customer-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.12);
}

.customer-header {
  display: flex;
  align-items: center;
  gap: 15px;
  margin-bottom: 20px;
}

.avatar {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 700;
  font-size: 1.2rem;
  flex-shrink: 0;
}

.customer-info {
  flex: 1;
}

.customer-name {
  font-size: 1.3rem;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 5px 0;
}

.customer-id {
  font-size: 0.85rem;
  color: #999;
  margin: 0;
}

/* Customer Contact */
.customer-contact {
  margin-bottom: 20px;
  padding-bottom: 20px;
  border-bottom: 1px solid #e0e0e0;
}

.contact-item {
  margin: 8px 0;
  font-size: 0.95rem;
  color: #666;
}

.contact-label {
  font-weight: 600;
  color: #333;
  margin-right: 8px;
}

.contact-value {
  color: #666;
}

/* Customer Stats */
.customer-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 15px;
  margin-bottom: 20px;
  padding: 15px;
  background: #f9f9f9;
  border-radius: 12px;
}

.stat {
  text-align: center;
}

.stat-label {
  display: block;
  font-size: 0.75rem;
  color: #999;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 5px;
  font-weight: 600;
}

.stat-value {
  display: block;
  font-size: 1.4rem;
  font-weight: 800;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Customer Actions */
.customer-actions {
  display: flex;
  gap: 10px;
}

.action-btn {
  flex: 1;
  padding: 10px 15px;
  border: none;
  border-radius: 10px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.view-btn {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  color: white;
}

.view-btn:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}

.edit-btn {
  background: rgba(160, 32, 240, 0.1);
  color: #a020f0;
}

.edit-btn:hover {
  background: rgba(160, 32, 240, 0.2);
  transform: translateY(-2px);
}

.delete-btn {
  background: rgba(255, 0, 110, 0.1);
  color: #ff006e;
}

.delete-btn:hover {
  background: rgba(255, 0, 110, 0.2);
  transform: translateY(-2px);
}

/* Empty State */
.empty-state {
  grid-column: 1 / -1;
  padding: 60px 20px;
  text-align: center;
  background: white;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
}

.empty-state p {
  font-size: 1.1rem;
  color: #999;
  margin: 0;
}

/* Loading State */
.loading-state {
  grid-column: 1 / -1;
  padding: 60px 20px;
  text-align: center;
  background: white;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
}

.loading-state p {
  font-size: 1.1rem;
  color: #a020f0;
  margin: 0;
  font-weight: 600;
}

/* Error State */
.error-state {
  grid-column: 1 / -1;
  padding: 60px 20px;
  text-align: center;
  background: white;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
}

.error-state p {
  font-size: 1.1rem;
  color: #ff006e;
  margin: 0 0 20px 0;
}

.retry-btn {
  padding: 12px 30px;
  border: none;
  border-radius: 10px;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  color: white;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.retry-btn:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 20px;
}

.modal-content {
  background: white;
  border-radius: 20px;
  padding: 30px;
  max-width: 500px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 50px rgba(0, 0, 0, 0.3);
  animation: modalSlideIn 0.3s ease;
}

@keyframes modalSlideIn {
  from {
    transform: translateY(-50px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
  padding-bottom: 15px;
  border-bottom: 2px solid #e0e0e0;
}

.modal-title {
  font-size: 1.8rem;
  font-weight: 800;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
}

.modal-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #999;
  cursor: pointer;
  padding: 5px 10px;
  transition: all 0.3s ease;
}

.modal-close:hover {
  color: #ff006e;
  transform: rotate(90deg);
}

/* Form */
.edit-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 0.9rem;
  font-weight: 600;
  color: #333;
}

.form-control {
  padding: 12px 15px;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.form-control:focus {
  outline: none;
  border-color: #a020f0;
  box-shadow: 0 0 0 3px rgba(160, 32, 240, 0.1);
}

.readonly-info {
  background: #f9f9f9;
  padding: 15px;
  border-radius: 10px;
}

.readonly-info label {
  color: #666;
  font-size: 0.85rem;
  margin-bottom: 5px;
}

.readonly-value {
  margin: 0;
  font-weight: 600;
  color: #333;
  font-size: 1rem;
}

.error-message {
  background: rgba(255, 0, 110, 0.1);
  color: #ff006e;
  padding: 12px 15px;
  border-radius: 10px;
  font-size: 0.9rem;
  border-left: 4px solid #ff006e;
}

.modal-actions {
  display: flex;
  gap: 15px;
  margin-top: 10px;
}

.btn-cancel,
.btn-save {
  flex: 1;
  padding: 12px 20px;
  border: none;
  border-radius: 10px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-cancel {
  background: #f0f0f0;
  color: #666;
}

.btn-cancel:hover:not(:disabled) {
  background: #e0e0e0;
}

.btn-save {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  color: white;
}

.btn-save:hover:not(:disabled) {
  opacity: 0.9;
  transform: translateY(-2px);
}

.btn-cancel:disabled,
.btn-save:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Responsive */
@media (max-width: 1024px) {
  .customers-list {
    grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  }
}

@media (max-width: 768px) {
  .gestion-clientes {
    padding: 20px 15px;
  }

  .page-title {
    font-size: 2rem;
  }

  .customers-list {
    grid-template-columns: 1fr;
  }

  .search-input {
    max-width: 100%;
  }

  .customer-card {
    padding: 20px;
  }

  .customer-stats {
    grid-template-columns: repeat(3, 1fr);
    gap: 10px;
    padding: 12px;
  }

  .stat-value {
    font-size: 1.1rem;
  }

  .action-btn {
    padding: 8px 12px;
    font-size: 0.8rem;
  }

  .modal-content {
    padding: 25px;
    max-width: 100%;
  }

  .modal-title {
    font-size: 1.5rem;
  }

  .modal-actions {
    flex-direction: column;
  }

  .btn-cancel,
  .btn-save {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.5rem;
  }

  .customer-header {
    gap: 12px;
  }

  .avatar {
    width: 50px;
    height: 50px;
    font-size: 1rem;
  }

  .customer-name {
    font-size: 1.1rem;
  }

  .customer-stats {
    grid-template-columns: repeat(3, 1fr);
    gap: 8px;
    padding: 10px;
  }

  .stat-label {
    font-size: 0.7rem;
  }

  .stat-value {
    font-size: 0.9rem;
  }

  .action-btn {
    padding: 6px 10px;
    font-size: 0.75rem;
  }

  .search-input {
    padding: 12px 15px;
    font-size: 0.9rem;
  }

  .modal-content {
    padding: 20px;
  }

  .modal-title {
    font-size: 1.3rem;
  }

  .form-control {
    padding: 10px 12px;
    font-size: 0.9rem;
  }
}
</style>
