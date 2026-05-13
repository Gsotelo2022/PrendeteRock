<template>
  <div class="gestion-productos">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="goHome">
        <span>←</span> Volver al Inicio
      </button>
      <div class="header-top">
        <div>
          <h1 class="page-title">Gestión de Productos</h1>
          <p class="page-subtitle">Gestiona tus productos de impresión e inventario</p>
        </div>
        <button class="add-product-btn" @click="handleAddProduct">
          <span>+</span> Agregar Producto
        </button>
      </div>
    </div>

    <!-- Products Grid -->
    <div class="products-grid">
      <div v-if="loading" class="loading-state">
        <p>Cargando productos...</p>
      </div>
      
      <div v-else-if="error" class="error-state">
        <p>{{ error }}</p>
        <button @click="loadProducts" class="retry-btn">Reintentar</button>
      </div>
      
      <div v-else-if="products.length === 0" class="empty-state">
        <p>No hay productos disponibles</p>
      </div>
      
      <div v-else v-for="product in products" :key="product.id" class="product-card">
        <div class="product-header">
          <h3 class="product-name">{{ product.name }}</h3>
          <p class="product-id">{{ product.id }}</p>
        </div>

        <div class="product-type">
          <span class="type-badge" :style="{ background: product.typeBg, color: product.typeColor }">
            {{ product.type }}
          </span>
        </div>

        <p class="product-description">{{ product.description }}</p>

        <div class="product-details">
          <div class="detail-item">
            <span class="detail-label">💵 Precio Base</span>
            <span class="detail-value">${{ product.basePrice }}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">❤️ Stock</span>
            <span class="detail-value">{{ product.stock }}</span>
          </div>
        </div>

        <div class="product-actions">
          <button class="action-btn edit-btn" @click="handleEdit(product.id)">
            <span>✎</span> Editar
          </button>
          <button class="action-btn delete-btn" @click="handleDelete(product.id)">
            <span>🗑</span> Eliminar
          </button>
        </div>
      </div>
    </div>

    <!-- Modal de Edición -->
    <div v-if="showEditModal" class="modal-overlay" @click="closeEditModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2 class="modal-title">Editar Producto</h2>
          <button class="modal-close" @click="closeEditModal">✕</button>
        </div>

        <form @submit.prevent="saveChanges" class="edit-form">
          <div class="form-group">
            <label for="name">Nombre del Producto</label>
            <input 
              v-model="editForm.name"
              type="text" 
              id="name"
              class="form-control"
              placeholder="Ingrese el nombre"
              required
            >
          </div>

          <div class="form-group">
            <label for="description">Descripción</label>
            <textarea 
              v-model="editForm.description"
              id="description"
              class="form-control"
              placeholder="Ingrese la descripción"
              rows="3"
              required
            ></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="basePrice">Precio Base</label>
              <input 
                v-model.number="editForm.basePrice"
                type="number" 
                id="basePrice"
                class="form-control"
                placeholder="0.00"
                step="0.01"
                min="0"
                required
              >
            </div>

            <div class="form-group">
              <label for="stock">Stock</label>
              <input 
                v-model.number="editForm.stock"
                type="number" 
                id="stock"
                class="form-control"
                placeholder="0"
                min="0"
                required
              >
            </div>
          </div>

          <div class="form-group">
            <label for="category">Categoría</label>
            <select 
              v-model="editForm.category"
              id="category"
              class="form-control"
              required
            >
              <option value="">Seleccionar categoría</option>
              <option value="Indumentaria">Indumentaria</option>
              <option value="Bazar y cocina">Bazar y cocina</option>
              <option value="Tazas y vasos">Tazas y vasos</option>
              <option value="Botellas térmicas y mates">Botellas térmicas y mates</option>
              <option value="Papelería">Papelería</option>
              <option value="Tecnología y accesorios">Tecnología y accesorios</option>
              <option value="Fundas para celulares">Fundas para celulares</option>
              <option value="Hogar y decoración">Hogar y decoración</option>
              <option value="Cuadros y posters">Cuadros y posters</option>
              <option value="Mochilas y bolsos">Mochilas y bolsos</option>
              <option value="Gorras y sombreros">Gorras y sombreros</option>
              <option value="Calzado">Calzado</option>
              <option value="Accesorios de moda">Accesorios de moda</option>
              <option value="Juguetería">Juguetería</option>
              <option value="Artículos deportivos">Artículos deportivos</option>
              <option value="Merchandising empresarial">Merchandising empresarial</option>
              <option value="Productos escolares">Productos escolares</option>
              <option value="Textiles para el hogar">Textiles para el hogar</option>
              <option value="Productos para mascotas">Productos para mascotas</option>
              <option value="Eventos y souvenirs">Eventos y souvenirs</option>
            </select>
          </div>

          <div class="form-group">
            <label for="imageUrl">URL de la Imagen</label>
            <input 
              v-model="editForm.imageUrl"
              type="text" 
              id="imageUrl"
              class="form-control"
              placeholder="/images/producto.jpg (opcional)"
            >
          </div>

          <div class="form-group">
            <label class="checkbox-label">
              <input 
                v-model="editForm.isActive"
                type="checkbox"
                class="form-checkbox"
              >
              <span>Producto activo</span>
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

    <!-- Modal de Agregar Producto -->
    <div v-if="showAddModal" class="modal-overlay" @click="closeAddModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2 class="modal-title">Agregar Nuevo Producto</h2>
          <button class="modal-close" @click="closeAddModal">✕</button>
        </div>

        <form @submit.prevent="createProduct" class="edit-form">
          <div class="form-group">
            <label for="addName">Nombre del Producto</label>
            <input 
              v-model="addForm.name"
              type="text" 
              id="addName"
              class="form-control"
              placeholder="Ingrese el nombre"
              required
            >
          </div>

          <div class="form-group">
            <label for="addDescription">Descripción</label>
            <textarea 
              v-model="addForm.description"
              id="addDescription"
              class="form-control"
              placeholder="Ingrese la descripción"
              rows="3"
              required
            ></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="addBasePrice">Precio Base</label>
              <input 
                v-model.number="addForm.basePrice"
                type="number" 
                id="addBasePrice"
                class="form-control"
                placeholder="0.00"
                step="0.01"
                min="0"
                required
              >
            </div>

            <div class="form-group">
              <label for="addStock">Stock</label>
              <input 
                v-model.number="addForm.stock"
                type="number" 
                id="addStock"
                class="form-control"
                placeholder="0"
                min="0"
                required
              >
            </div>
          </div>

          <div class="form-group">
            <label for="addCategory">Categoría</label>
            <select 
              v-model="addForm.category"
              id="addCategory"
              class="form-control"
              required
            >
              <option value="">Seleccionar categoría</option>
              <option value="Indumentaria">Indumentaria</option>
              <option value="Bazar y cocina">Bazar y cocina</option>
              <option value="Tazas y vasos">Tazas y vasos</option>
              <option value="Botellas térmicas y mates">Botellas térmicas y mates</option>
              <option value="Papelería">Papelería</option>
              <option value="Tecnología y accesorios">Tecnología y accesorios</option>
              <option value="Fundas para celulares">Fundas para celulares</option>
              <option value="Hogar y decoración">Hogar y decoración</option>
              <option value="Cuadros y posters">Cuadros y posters</option>
              <option value="Mochilas y bolsos">Mochilas y bolsos</option>
              <option value="Gorras y sombreros">Gorras y sombreros</option>
              <option value="Calzado">Calzado</option>
              <option value="Accesorios de moda">Accesorios de moda</option>
              <option value="Juguetería">Juguetería</option>
              <option value="Artículos deportivos">Artículos deportivos</option>
              <option value="Merchandising empresarial">Merchandising empresarial</option>
              <option value="Productos escolares">Productos escolares</option>
              <option value="Textiles para el hogar">Textiles para el hogar</option>
              <option value="Productos para mascotas">Productos para mascotas</option>
              <option value="Eventos y souvenirs">Eventos y souvenirs</option>
            </select>
          </div>

          <div class="form-group">
            <label for="addImageUrl">URL de la Imagen</label>
            <input 
              v-model="addForm.imageUrl"
              type="text" 
              id="addImageUrl"
              class="form-control"
              placeholder="/images/producto.jpg (opcional)"
            >
          </div>

          <div class="form-group">
            <label class="checkbox-label">
              <input 
                v-model="addForm.isActive"
                type="checkbox"
                class="form-checkbox"
              >
              <span>Producto activo</span>
            </label>
          </div>

          <div v-if="addError" class="error-message">
            {{ addError }}
          </div>

          <div class="modal-actions">
            <button type="button" class="btn-cancel" @click="closeAddModal" :disabled="saving">
              Cancelar
            </button>
            <button type="submit" class="btn-save" :disabled="saving">
              {{ saving ? 'Creando...' : 'Crear Producto' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal de Confirmación de Eliminación -->
    <div v-if="showDeleteModal" class="modal-overlay" @click="closeDeleteModal">
      <div class="modal-content delete-modal" @click.stop>
        <div class="modal-header">
          <h2 class="modal-title">Confirmar Eliminación</h2>
          <button class="modal-close" @click="closeDeleteModal">✕</button>
        </div>

        <div class="delete-content">
          <div class="delete-icon">⚠️</div>
          <p class="delete-message">
            ¿Estás seguro de que deseas eliminar este producto?
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
            {{ deleting ? 'Eliminando...' : 'Eliminar Producto' }}
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
  name: 'GestionProductos',
  setup() {
    const router = useRouter()
    const api = useApi()
    const toast = useToast()
    return { router, api, toast }
  },
  data() {
    return {
      products: [],
      loading: true,
      error: null,
      showEditModal: false,
      showAddModal: false,
      showDeleteModal: false,
      productToDelete: null,
      editForm: {
        id: null,
        name: '',
        description: '',
        basePrice: 0,
        category: '',
        imageUrl: '',
        isActive: true,
        stock: 0
      },
      addForm: {
        name: '',
        description: '',
        basePrice: 0,
        category: '',
        imageUrl: '',
        isActive: true,
        stock: 0
      },
      saving: false,
      deleting: false,
      editError: null,
      addError: null
    }
  },
  async mounted() {
    await this.loadProducts()
  },
  methods: {
    // Cargar productos desde la API
    async loadProducts() {
      try {
        this.loading = true
        this.error = null
        const response = await this.api.get('/products')
        
        // Transformar datos del backend al formato del frontend
        this.products = response.map((product, index) => ({
          id: product.id,
          productId: `PROD-${String(product.id).padStart(3, '0')}`,
          name: product.name,
          type: product.category || 'General',
          typeBg: this.getCategoryBg(product.category),
          typeColor: this.getCategoryColor(product.category),
          description: product.description,
          basePrice: product.basePrice.toFixed(2),
          stock: product.stock || 0,
          createdAt: product.createdAt
        }))
      } catch (error) {
        console.error('Error al cargar productos:', error)
        this.error = 'Error al cargar los productos'
      } finally {
        this.loading = false
      }
    },

    // Obtener color de fondo según categoría
    getCategoryBg(category) {
      const colors = {
        'Paper': 'rgba(167, 139, 250, 0.2)',
        'Canvas': 'rgba(236, 72, 153, 0.2)',
        'Metal': 'rgba(59, 130, 246, 0.2)',
        'Acrylic': 'rgba(34, 197, 94, 0.2)',
        'Wood': 'rgba(251, 146, 60, 0.2)',
        'Fabric': 'rgba(244, 114, 182, 0.2)'
      }
      return colors[category] || 'rgba(156, 163, 175, 0.2)'
    },

    // Obtener color de texto según categoría
    getCategoryColor(category) {
      const colors = {
        'Paper': '#6d28d9',
        'Canvas': '#be185d',
        'Metal': '#1e40af',
        'Acrylic': '#15803d',
        'Wood': '#c2410c',
        'Fabric': '#db2777'
      }
      return colors[category] || '#6b7280'
    },

    goHome() {
      this.router.push('/admin')
    },
    handleAddProduct() {
      this.addForm = {
        name: '',
        description: '',
        basePrice: 0,
        category: '',
        imageUrl: '',
        isActive: true,
        stock: 0
      }
      this.addError = null
      this.showAddModal = true
    },
    async handleEdit(productId) {
      try {
        // Obtener el producto actual de la lista
        const product = this.products.find(p => p.id === productId)
        if (!product) {
          this.toast.error('Producto no encontrado')
          return
        }

        // Obtener datos completos del producto desde la API
        const fullProduct = await this.api.get(`/products/${productId}`)
        
        this.editForm = {
          id: fullProduct.id,
          name: fullProduct.name,
          description: fullProduct.description,
          basePrice: fullProduct.basePrice,
          category: fullProduct.category,
          imageUrl: fullProduct.imageUrl || '',
          isActive: fullProduct.isActive,
          stock: fullProduct.stock || 0
        }
        this.editError = null
        this.showEditModal = true
      } catch (error) {
        console.error('Error al cargar producto:', error)
        this.toast.error('Error al cargar los datos del producto')
      }
    },
    closeEditModal() {
      this.showEditModal = false
      this.editError = null
      this.editForm = {
        id: null,
        name: '',
        description: '',
        basePrice: 0,
        category: '',
        imageUrl: '',
        isActive: true,
        stock: 0
      }
    },
    async saveChanges() {
      try {
        this.saving = true
        this.editError = null

        await this.api.put(`/products/${this.editForm.id}`, {
          name: this.editForm.name,
          description: this.editForm.description,
          basePrice: this.editForm.basePrice,
          category: this.editForm.category,
          imageUrl: this.editForm.imageUrl || null,
          isActive: this.editForm.isActive,
          stock: this.editForm.stock
        })

        // Recargar lista de productos
        await this.loadProducts()
        
        // Cerrar modal
        this.closeEditModal()
        
        this.toast.success('Producto actualizado exitosamente')
      } catch (error) {
        console.error('Error al actualizar producto:', error)
        this.editError = 'Error al actualizar el producto. Por favor, intente nuevamente.'
        this.toast.error('Error al actualizar el producto')
      } finally {
        this.saving = false
      }
    },
    closeAddModal() {
      this.showAddModal = false
      this.addError = null
      this.addForm = {
        name: '',
        description: '',
        basePrice: 0,
        category: '',
        imageUrl: '',
        isActive: true,
        stock: 0
      }
    },
    async createProduct() {
      try {
        this.saving = true
        this.addError = null

        await this.api.post('/products', {
          name: this.addForm.name,
          description: this.addForm.description,
          basePrice: this.addForm.basePrice,
          category: this.addForm.category,
          imageUrl: this.addForm.imageUrl || null,
          isActive: this.addForm.isActive,
          stock: this.addForm.stock
        })

        // Recargar lista de productos
        await this.loadProducts()
        
        // Cerrar modal
        this.closeAddModal()
        
        this.toast.success('Producto creado exitosamente')
      } catch (error) {
        console.error('Error al crear producto:', error)
        this.addError = 'Error al crear el producto. Por favor, intente nuevamente.'
        this.toast.error('Error al crear el producto')
      } finally {
        this.saving = false
      }
    },
    async handleDelete(productId) {
      this.productToDelete = productId
      this.showDeleteModal = true
    },
    closeDeleteModal() {
      this.showDeleteModal = false
      this.productToDelete = null
    },
    async confirmDelete() {
      if (!this.productToDelete) return
      
      try {
        this.deleting = true
        await this.api.del(`/products/${this.productToDelete}`)
        this.toast.success('Producto eliminado exitosamente')
        await this.loadProducts()
        this.closeDeleteModal()
      } catch (error) {
        console.error('Error al eliminar producto:', error)
        this.toast.error('Error al eliminar el producto')
      } finally {
        this.deleting = false
      }
    }
  }
}
</script>

<style scoped>
.gestion-productos {
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

.add-product-btn {
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

.add-product-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(160, 32, 240, 0.3);
}

.add-product-btn span {
  font-size: 1.3rem;
}

/* Products Grid */
.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 25px;
}

.loading-state,
.error-state,
.empty-state {
  grid-column: 1 / -1;
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

.product-card {
  background: white;
  padding: 25px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
}

.product-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.12);
}

.product-header {
  margin-bottom: 12px;
}

.product-name {
  font-size: 1.3rem;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 5px 0;
}

.product-id {
  font-size: 0.85rem;
  color: #999;
  margin: 0;
}

/* Product Type */
.product-type {
  margin-bottom: 15px;
}

.type-badge {
  display: inline-block;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
}

/* Product Description */
.product-description {
  color: #666;
  font-size: 0.95rem;
  margin-bottom: 20px;
  flex: 1;
  line-height: 1.5;
}

/* Product Details */
.product-details {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding: 15px;
  background: #f9f9f9;
  border-radius: 12px;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.detail-label {
  font-size: 0.8rem;
  color: #999;
  font-weight: 600;
}

.detail-value {
  font-size: 1.2rem;
  font-weight: 700;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Product Actions */
.product-actions {
  display: flex;
  gap: 10px;
}

.action-btn {
  flex: 1;
  padding: 10px 15px;
  border: none;
  border-radius: 10px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
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

.edit-form {
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

textarea.form-control {
  resize: vertical;
  min-height: 80px;
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
  .products-grid {
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  }

  .header-top {
    flex-direction: column;
    align-items: flex-start;
  }

  .add-product-btn {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 768px) {
  .gestion-productos {
    padding: 20px 15px;
  }

  .page-title {
    font-size: 2rem;
  }

  .products-grid {
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 20px;
  }

  .product-card {
    padding: 20px;
  }

  .product-details {
    padding: 12px;
    gap: 10px;
  }

  .detail-label {
    font-size: 0.75rem;
  }

  .detail-value {
    font-size: 1rem;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.5rem;
  }

  .products-grid {
    grid-template-columns: 1fr;
    gap: 15px;
  }

  .product-card {
    padding: 15px;
  }

  .product-name {
    font-size: 1.1rem;
  }

  .product-description {
    font-size: 0.85rem;
    margin-bottom: 15px;
  }

  .product-details {
    flex-direction: column;
    gap: 8px;
    padding: 10px;
  }

  .detail-item {
    width: 100%;
    justify-content: space-between;
  }

  .action-btn {
    padding: 8px 12px;
    font-size: 0.8rem;
  }

  .add-product-btn {
    padding: 12px 18px;
    font-size: 0.9rem;
  }

  .add-product-btn span {
    font-size: 1.1rem;
  }
}
</style>
