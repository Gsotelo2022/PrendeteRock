<template>
  <div class="gestion-cupones">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="goHome">
        <span>←</span> Volver al Inicio
      </button>
      <div class="header-top">
        <div>
          <h1 class="page-title">Gestión de Cupones</h1>
          <p class="page-subtitle">Crea y gestiona cupones promocionales</p>
        </div>
        <button class="create-coupon-btn" @click="handleCreateCoupon">
          <span>+</span> Crear Cupón
        </button>
      </div>
    </div>

    <!-- Loading / Error / Empty States -->
    <div v-if="loading" class="loading-state">
      <p>Cargando cupones...</p>
    </div>

    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button @click="loadCoupons" class="retry-btn">Reintentar</button>
    </div>

    <div v-else-if="coupons.length === 0" class="empty-state">
      <p>No hay cupones disponibles</p>
    </div>

    <!-- Coupons List -->
    <div v-else class="coupons-list">
      <div v-for="coupon in coupons" :key="coupon.id" class="coupon-card">
        <div class="coupon-header">
          <div class="coupon-code" :style="{ background: coupon.codeBg }">
            {{ coupon.code }}
          </div>
          <button class="copy-btn" @click="handleCopyCoupon(coupon.code)">
            📋
          </button>
          <span class="status-badge" :class="'status-' + coupon.status.toLowerCase()">
            {{ coupon.status }}
          </span>
        </div>

        <div class="coupon-details">
          <div class="detail-row">
            <div class="detail-item">
              <span class="detail-label">% Porcentaje</span>
              <span class="detail-value">{{ coupon.discount }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">📅 Vencimiento</span>
              <span class="detail-value">{{ coupon.expiryDate }}</span>
            </div>
          </div>

          <div class="detail-row">
            <div class="detail-item">
              <span class="detail-label">🎯 Usos</span>
              <span class="detail-value">{{ coupon.usageCount }} / {{ coupon.usageLimit }}</span>
            </div>
            <div class="detail-progress">
              <div class="progress-bar">
                <div class="progress-fill" :style="{ width: coupon.progressPercent + '%' }"></div>
              </div>
            </div>
          </div>
        </div>

        <div class="coupon-actions">
          <button class="action-btn edit-btn" @click="handleEdit(coupon)">
            ✏️ Editar
          </button>
          <button 
            v-if="coupon.status === 'Active'"
            class="action-btn deactivate-btn" 
            @click="handleToggleStatus(coupon.id)"
          >
            Desactivar
          </button>
          <button 
            v-else
            class="action-btn activate-btn" 
            @click="handleToggleStatus(coupon.id)"
          >
            Activar
          </button>
          <button class="action-btn delete-btn" @click="handleDelete(coupon.id)">
            🗑 Eliminar
          </button>
        </div>
      </div>
    </div>

    <!-- Modal de Crear Cupón -->
    <div v-if="showCreateModal" class="modal-overlay" @click="closeCreateModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2 class="modal-title">Crear Nuevo Cupón</h2>
          <button class="modal-close" @click="closeCreateModal">✕</button>
        </div>

        <form @submit.prevent="createCoupon" class="coupon-form">
          <div class="form-group">
            <label for="code">Código del Cupón</label>
            <input 
              v-model="createForm.code"
              type="text" 
              id="code"
              class="form-control"
              placeholder="Ej: VERANO2026"
              required
              maxlength="20"
              @input="createForm.code = createForm.code.toUpperCase()"
            >
            <small class="form-hint">El código se convertirá automáticamente a mayúsculas</small>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="percentage">Porcentaje de Descuento</label>
              <div class="input-with-addon">
                <input 
                  v-model.number="createForm.percentage"
                  type="number" 
                  id="percentage"
                  class="form-control"
                  placeholder="20"
                  step="0.01"
                  min="0"
                  max="100"
                  required
                >
                <span class="input-addon">%</span>
              </div>
            </div>

            <div class="form-group">
              <label for="usageLimit">Límite de Usos</label>
              <input 
                v-model.number="createForm.usageLimit"
                type="number" 
                id="usageLimit"
                class="form-control"
                placeholder="100 (dejar vacío para ilimitado)"
                min="1"
              >
            </div>
          </div>

          <div class="form-group">
            <label for="expiryDate">Fecha de Vencimiento</label>
            <input 
              v-model="createForm.expiryDate"
              type="date" 
              id="expiryDate"
              class="form-control"
              :min="minDate"
            >
            <small class="form-hint">Dejar vacío para cupón sin vencimiento</small>
          </div>

          <div v-if="createError" class="error-message">
            {{ createError }}
          </div>

          <div class="modal-actions">
            <button type="button" class="btn-cancel" @click="closeCreateModal" :disabled="saving">
              Cancelar
            </button>
            <button type="submit" class="btn-save" :disabled="saving">
              {{ saving ? 'Creando...' : 'Crear Cupón' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal de Editar Cupón -->
    <div v-if="showEditModal" class="modal-overlay" @click="closeEditModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2 class="modal-title">Editar Cupón</h2>
          <button class="modal-close" @click="closeEditModal">✕</button>
        </div>

        <form class="coupon-form" @submit.prevent="saveEditChanges">
          <div class="form-group">
            <label for="edit-code">Código del Cupón</label>
            <input 
              v-model="editForm.code"
              type="text" 
              id="edit-code"
              class="form-control"
              placeholder="VERANO2026"
              maxlength="20"
              @input="editForm.code = editForm.code.toUpperCase()"
              required
            >
            <small class="form-hint">Código único en mayúsculas</small>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="edit-percentage">Porcentaje de Descuento</label>
              <div class="input-with-addon">
                <input 
                  v-model.number="editForm.percentage"
                  type="number" 
                  id="edit-percentage"
                  class="form-control"
                  placeholder="20"
                  step="0.01"
                  min="0"
                  max="100"
                  required
                >
                <span class="input-addon">%</span>
              </div>
            </div>

            <div class="form-group">
              <label for="edit-usageLimit">Límite de Usos</label>
              <input 
                v-model.number="editForm.usageLimit"
                type="number" 
                id="edit-usageLimit"
                class="form-control"
                placeholder="100 (dejar vacío para ilimitado)"
                min="1"
              >
            </div>
          </div>

          <div class="form-group">
            <label for="edit-expiryDate">Fecha de Vencimiento</label>
            <input 
              v-model="editForm.expiryDate"
              type="date" 
              id="edit-expiryDate"
              class="form-control"
              :min="minDate"
            >
            <small class="form-hint">Dejar vacío para cupón sin vencimiento</small>
          </div>

          <div class="form-group">
            <label class="checkbox-label">
              <input 
                v-model="editForm.isActive"
                type="checkbox"
                class="form-checkbox"
              >
              <span>Cupón activo</span>
            </label>
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

    <!-- Modal de confirmación de eliminación -->
    <div v-if="showDeleteModal" class="modal-overlay" @click="closeDeleteModal">
      <div class="modal-content delete-modal" @click.stop>
        <div class="modal-header">
          <h2 class="modal-title">Confirmar Eliminación</h2>
          <button class="modal-close" @click="closeDeleteModal">✕</button>
        </div>

        <div class="delete-content">
          <div class="delete-icon">⚠️</div>
          <p class="delete-message">
            ¿Estás seguro de que deseas eliminar este cupón?
          </p>
          <p class="delete-warning">
            Esta acción no se puede deshacer.
          </p>
        </div>

        <div class="modal-actions">
          <button type="button" class="btn-cancel" @click="closeDeleteModal" :disabled="deleting">
            Cancelar
          </button>
          <button type="button" class="btn-delete" @click="confirmDelete" :disabled="deleting">
            {{ deleting ? 'Eliminando...' : 'Eliminar Cupón' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useRouter } from 'vue-router'
import { useApi } from '../../composables/useApi'
import { useToast } from '../../composables/useToast'

export default {
  name: 'GestionCupones',
  setup() {
    const router = useRouter()
    const api = useApi()
    const toast = useToast()
    return { router, api, toast }
  },
  data() {
    return {
      coupons: [],
      loading: true,
      error: null,
      showCreateModal: false,
      createForm: {
        code: '',
        percentage: null,
        usageLimit: null,
        expiryDate: ''
      },
      saving: false,
      createError: null,
      showEditModal: false,
      editForm: {
        id: null,
        code: '',
        percentage: null,
        usageLimit: null,
        expiryDate: '',
        isActive: true
      },
      editError: null,
      showDeleteModal: false,
      couponToDelete: null,
      deleting: false
    }
  },
  computed: {
    minDate() {
      // Fecha mínima es hoy
      const today = new Date()
      return today.toISOString().split('T')[0]
    }
  },
  async mounted() {
    await this.loadCoupons()
  },
  methods: {
    // Cargar cupones desde la API
    async loadCoupons() {
      try {
        this.loading = true
        this.error = null
        const response = await this.api.get('/coupons')
        
        // Transformar datos del backend al formato del frontend
        this.coupons = response.map((coupon, index) => {
          const progressPercent = coupon.usageLimit 
            ? Math.round((coupon.usageCount / coupon.usageLimit) * 100)
            : 0
          
          const expiryDate = coupon.expiryDate 
            ? new Date(coupon.expiryDate).toLocaleDateString('es-ES')
            : 'Sin vencimiento'
          
          return {
            id: coupon.id,
            code: coupon.code,
            codeBg: this.getCodeGradient(index),
            status: coupon.isActive ? 'Active' : 'Inactive',
            discount: `${coupon.percentage}%`,
            expiryDate: expiryDate,
            usageCount: coupon.usageCount,
            usageLimit: coupon.usageLimit || 'Ilimitado',
            progressPercent: progressPercent
          }
        })
      } catch (error) {
        console.error('Error al cargar cupones:', error)
        this.error = 'Error al cargar los cupones'
      } finally {
        this.loading = false
      }
    },

    // Obtener gradiente para el código
    getCodeGradient(index) {
      const gradients = [
        'linear-gradient(135deg, #a020f0 0%, #ff006e 100%)',
        'linear-gradient(135deg, #ff006e 0%, #ff7a00 100%)',
        'linear-gradient(135deg, #a020f0 0%, #ff7a00 100%)',
        'linear-gradient(135deg, #ff7a00 0%, #a020f0 100%)'
      ]
      return gradients[index % gradients.length]
    },

    goHome() {
      this.router.push('/admin')
    },
    handleCreateCoupon() {
      this.createForm = {
        code: '',
        percentage: null,
        usageLimit: null,
        expiryDate: ''
      }
      this.createError = null
      this.showCreateModal = true
    },
    closeCreateModal() {
      this.showCreateModal = false
      this.createError = null
      this.createForm = {
        code: '',
        percentage: null,
        usageLimit: null,
        expiryDate: ''
      }
    },
    async createCoupon() {
      try {
        this.saving = true
        this.createError = null

        // Preparar datos para enviar
        const data = {
          code: this.createForm.code,
          percentage: this.createForm.percentage,
          usageLimit: this.createForm.usageLimit || null,
          expiryDate: this.createForm.expiryDate || null
        }

        await this.api.post('/coupons', data)
        
        // Recargar lista de cupones
        await this.loadCoupons()
        
        // Cerrar modal
        this.closeCreateModal()
        
        this.toast.success('Cupón creado exitosamente')
      } catch (error) {
        console.error('Error al crear cupón:', error)
        this.createError = 'Error al crear el cupón. Verifica que el código no exista ya.'
        this.toast.error('Error al crear el cupón')
      } finally {
        this.saving = false
      }
    },
    handleCopyCoupon(code) {
      navigator.clipboard.writeText(code)
      this.toast.success(`Código copiado: ${code}`)
    },
    async handleEdit(coupon) {
      // Cargar datos del cupón en el formulario de edición
      const originalCoupon = this.coupons.find(c => c.id === coupon.id)
      if (!originalCoupon) return

      // Obtener el cupón completo de la API
      try {
        const fullCoupon = await this.api.get(`/coupons/${coupon.id}`)
        
        this.editForm = {
          id: fullCoupon.id,
          code: fullCoupon.code,
          percentage: fullCoupon.percentage,
          usageLimit: fullCoupon.usageLimit,
          expiryDate: fullCoupon.expiryDate ? fullCoupon.expiryDate.split('T')[0] : '',
          isActive: fullCoupon.isActive
        }
        this.editError = null
        this.showEditModal = true
      } catch (error) {
        console.error('Error al cargar cupón:', error)
        this.toast.error('Error al cargar los datos del cupón')
      }
    },
    closeEditModal() {
      this.showEditModal = false
      this.editForm = {
        id: null,
        code: '',
        percentage: null,
        usageLimit: null,
        expiryDate: '',
        isActive: true
      }
      this.editError = null
    },
    async saveEditChanges() {
      if (!this.editForm.id) return
      
      this.saving = true
      this.editError = null

      try {
        const data = {
          code: this.editForm.code,
          percentage: this.editForm.percentage,
          usageLimit: this.editForm.usageLimit || null,
          expiryDate: this.editForm.expiryDate || null,
          isActive: this.editForm.isActive
        }

        await this.api.put(`/coupons/${this.editForm.id}`, data)
        await this.loadCoupons()
        this.closeEditModal()
        this.toast.success('Cupón actualizado exitosamente')
      } catch (error) {
        console.error('Error al actualizar cupón:', error)
        this.editError = 'Error al actualizar el cupón. Verifica que el código no esté duplicado.'
        this.toast.error('Error al actualizar el cupón')
      } finally {
        this.saving = false
      }
    },
    async handleToggleStatus(couponId) {
      try {
        await this.api.patch(`/coupons/${couponId}/toggle-status`, {})
        this.toast.success('Estado del cupón actualizado')
        await this.loadCoupons()
      } catch (error) {
        console.error('Error al cambiar estado del cupón:', error)
        this.toast.error('Error al actualizar el estado')
      }
    },
    async handleDelete(couponId) {
      this.couponToDelete = couponId
      this.showDeleteModal = true
    },
    closeDeleteModal() {
      this.showDeleteModal = false
      this.couponToDelete = null
    },
    async confirmDelete() {
      if (!this.couponToDelete) return
      
      try {
        this.deleting = true
        await this.api.del(`/coupons/${this.couponToDelete}`)
        this.toast.success('Cupón eliminado exitosamente')
        await this.loadCoupons()
        this.closeDeleteModal()
      } catch (error) {
        console.error('Error al eliminar cupón:', error)
        this.toast.error('Error al eliminar el cupón')
      } finally {
        this.deleting = false
      }
    }
  }
}
</script>

<style scoped>
.gestion-cupones {
  padding: 40px 0;
  background-color: #faf7f2;
  min-height: 100vh;
}

/* Header */
.header {
  margin-bottom: 40px;
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

.header-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
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

.create-coupon-btn {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  color: white;
  border: none;
  padding: 15px 25px;
  border-radius: 15px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 8px;
  white-space: nowrap;
}

.create-coupon-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(160, 32, 240, 0.3);
}

.create-coupon-btn span {
  font-size: 1.3rem;
}

/* Loading / Error / Empty States */
.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 60px 20px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
}

.loading-state p,
.error-state p,
.empty-state p {
  font-size: 1.2rem;
  color: #666;
  margin: 0;
}

.retry-btn {
  margin-top: 20px;
  padding: 12px 24px;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.retry-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(160, 32, 240, 0.3);
}

/* Coupons List */
.coupons-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.coupon-card {
  background: white;
  padding: 25px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.coupon-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.12);
}

/* Coupon Header */
.coupon-header {
  display: flex;
  align-items: center;
  gap: 15px;
  margin-bottom: 20px;
}

.coupon-code {
  padding: 12px 20px;
  border-radius: 12px;
  color: white;
  font-weight: 700;
  font-size: 1rem;
  letter-spacing: 1px;
  flex-shrink: 0;
}

.copy-btn {
  background: none;
  border: none;
  font-size: 1.3rem;
  cursor: pointer;
  padding: 5px;
  transition: transform 0.2s;
}

.copy-btn:hover {
  transform: scale(1.2);
}

.status-badge {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
  margin-left: auto;
}

.status-active {
  background: rgba(34, 197, 94, 0.2);
  color: #15803d;
}

.status-inactive {
  background: rgba(107, 114, 128, 0.2);
  color: #374151;
}

/* Coupon Details */
.coupon-details {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  margin-bottom: 20px;
  padding: 20px;
  background: #f9f9f9;
  border-radius: 15px;
}

.detail-row {
  display: flex;
  gap: 15px;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 5px;
  flex: 1;
}

.detail-label {
  font-size: 0.75rem;
  color: #999;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-weight: 600;
}

.detail-value {
  font-size: 1rem;
  font-weight: 700;
  color: #1a1a1a;
}

.detail-progress {
  display: flex;
  flex-direction: column;
  gap: 5px;
  flex: 1;
}

.progress-bar {
  height: 8px;
  background: #e0e0e0;
  border-radius: 10px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  transition: width 0.3s ease;
}

/* Coupon Actions */
.coupon-actions {
  display: flex;
  gap: 12px;
}

.action-btn {
  flex: 1;
  padding: 12px 15px;
  border: none;
  border-radius: 10px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.edit-btn {
  background: rgba(160, 32, 240, 0.1);
  color: #a020f0;
}

.edit-btn:hover {
  background: rgba(160, 32, 240, 0.2);
  transform: translateY(-2px);
}

.deactivate-btn {
  background: rgba(100, 116, 139, 0.1);
  color: #475569;
}

.deactivate-btn:hover {
  background: rgba(100, 116, 139, 0.2);
  transform: translateY(-2px);
}

.activate-btn {
  background: rgba(34, 197, 94, 0.1);
  color: #15803d;
}

.activate-btn:hover {
  background: rgba(34, 197, 94, 0.2);
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

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  backdrop-filter: blur(5px);
}

.modal-content {
  background: white;
  border-radius: 25px;
  padding: 40px;
  max-width: 600px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  animation: modalSlideIn 0.3s ease;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-50px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.modal-title {
  font-size: 1.8rem;
  font-weight: 700;
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
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.modal-close:hover {
  color: #ff006e;
  transform: rotate(90deg);
}

/* Form Styles */
.coupon-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 600;
  color: #333;
  font-size: 0.95rem;
}

.form-control {
  padding: 12px 16px;
  border: 2px solid #e5e5e5;
  border-radius: 12px;
  font-size: 1rem;
  transition: all 0.3s ease;
  font-family: inherit;
}

.form-control:focus {
  outline: none;
  border-color: #a020f0;
  box-shadow: 0 0 0 3px rgba(160, 32, 240, 0.1);
}

.form-hint {
  font-size: 0.85rem;
  color: #999;
  font-style: italic;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  user-select: none;
}

.form-checkbox {
  width: 20px;
  height: 20px;
  cursor: pointer;
  accent-color: #a020f0;
}

.checkbox-label span {
  font-weight: 600;
  color: #333;
}

.input-with-addon {
  position: relative;
  display: flex;
  align-items: center;
}

.input-with-addon .form-control {
  flex: 1;
  padding-right: 45px;
}

.input-addon {
  position: absolute;
  right: 16px;
  font-weight: 600;
  color: #a020f0;
  font-size: 1.1rem;
}

.error-message {
  background: rgba(255, 0, 110, 0.1);
  color: #ff006e;
  padding: 12px;
  border-radius: 10px;
  font-size: 0.9rem;
}

.modal-actions {
  display: flex;
  gap: 15px;
  margin-top: 10px;
}

.btn-cancel,
.btn-save {
  flex: 1;
  padding: 14px 20px;
  border: none;
  border-radius: 12px;
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
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(160, 32, 240, 0.3);
}

.btn-cancel:disabled,
.btn-save:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Delete Modal Styles */
.delete-modal {
  max-width: 450px;
}

.delete-content {
  text-align: center;
  padding: 20px 0;
}

.delete-icon {
  font-size: 4rem;
  margin-bottom: 20px;
  animation: shake 0.5s ease;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-10px); }
  75% { transform: translateX(10px); }
}

.delete-message {
  font-size: 1.1rem;
  color: #333;
  font-weight: 600;
  margin: 0 0 10px 0;
}

.delete-warning {
  font-size: 0.95rem;
  color: #999;
  margin: 0;
}

.btn-delete {
  flex: 1;
  padding: 14px 20px;
  border: none;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  background: linear-gradient(135deg, #ff006e 0%, #ff4d4d 100%);
  color: white;
}

.btn-delete:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(255, 0, 110, 0.4);
}

.btn-delete:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Responsive */
@media (max-width: 1024px) {
  .coupon-details {
    grid-template-columns: 1fr;
  }

  .header-top {
    flex-direction: column;
    align-items: flex-start;
  }

  .create-coupon-btn {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 768px) {
  .gestion-cupones {
    padding: 20px 15px;
  }

  .page-title {
    font-size: 2rem;
  }

  .coupon-card {
    padding: 20px;
  }

  .coupon-header {
    flex-wrap: wrap;
    gap: 12px;
  }

  .status-badge {
    margin-left: auto;
  }

  .coupon-details {
    gap: 15px;
    padding: 15px;
  }

  .action-btn {
    padding: 10px 12px;
    font-size: 0.8rem;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.5rem;
  }

  .coupon-code {
    padding: 10px 15px;
    font-size: 0.85rem;
  }

  .coupon-header {
    gap: 10px;
  }

  .coupon-details {
    grid-template-columns: 1fr;
    gap: 12px;
    padding: 12px;
  }

  .detail-row {
    gap: 12px;
  }

  .action-btn {
    padding: 8px 10px;
    font-size: 0.75rem;
  }

  .create-coupon-btn {
    padding: 12px 18px;
    font-size: 0.9rem;
  }

  .create-coupon-btn span {
    font-size: 1.1rem;
  }
}
</style>
