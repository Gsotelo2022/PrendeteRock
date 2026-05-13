<template>
  <div class="gestion-productos">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="goHome">
        <span>←</span> Volver al Inicio
      </button>
      <div class="header-top">
        <div>
          <h1 class="page-title">Products Management</h1>
          <p class="page-subtitle">Manage your print products and inventory</p>
        </div>
        <button class="add-product-btn" @click="handleAddProduct">
          <span>+</span> Add Product
        </button>
      </div>
    </div>

    <!-- Products Grid -->
    <div class="products-grid">
      <div v-for="product in products" :key="product.id" class="product-card">
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
            <span class="detail-label">💵 Base Price</span>
            <span class="detail-value">${{ product.basePrice }}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">❤️ Stock</span>
            <span class="detail-value">{{ product.stock }}</span>
          </div>
        </div>

        <div class="product-actions">
          <button class="action-btn edit-btn" @click="handleEdit(product.id)">
            <span>✎</span> Edit
          </button>
          <button class="action-btn delete-btn" @click="handleDelete(product.id)">
            <span>🗑</span> Delete
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useRouter } from 'vue-router'

export default {
  name: 'GestionProductos',
  setup() {
    const router = useRouter()
    return { router }
  },
  data() {
    return {
      products: [
        {
          id: 'PROD-001',
          name: 'Poster Print',
          type: 'Paper',
          typeBg: 'rgba(167, 139, 250, 0.2)',
          typeColor: '#6d28d9',
          description: 'High-quality poster on premium paper',
          basePrice: '24.99',
          stock: 150
        },
        {
          id: 'PROD-002',
          name: 'Canvas Print',
          type: 'Canvas',
          typeBg: 'rgba(236, 72, 153, 0.2)',
          typeColor: '#be185d',
          description: 'Gallery-wrapped canvas with wooden frame',
          basePrice: '44.99',
          stock: 85
        },
        {
          id: 'PROD-003',
          name: 'Metal Print',
          type: 'Metal',
          typeBg: 'rgba(59, 130, 246, 0.2)',
          typeColor: '#1e40af',
          description: 'Vibrant metal print with glossy finish',
          basePrice: '79.99',
          stock: 45
        },
        {
          id: 'PROD-004',
          name: 'Acrylic Print',
          type: 'Acrylic',
          typeBg: 'rgba(34, 197, 94, 0.2)',
          typeColor: '#15803d',
          description: 'Modern acrylic print with depth and shine',
          basePrice: '99.99',
          stock: 30
        }
      ]
    }
  },
  methods: {
    goHome() {
      this.router.push('/admin')
    },
    handleAddProduct() {
      console.log('Agregar nuevo producto')
      // TODO: Implementar modal para agregar producto
    },
    handleEdit(productId) {
      console.log('Editar producto:', productId)
      // TODO: Implementar edición de producto
    },
    handleDelete(productId) {
      if (confirm('¿Estás seguro de que deseas eliminar este producto?')) {
        console.log('Eliminar producto:', productId)
        // TODO: Implementar eliminación de producto
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
