<template>
  <div class="my-orders">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="goHome">
        <span>←</span> Volver al Inicio
      </button>
      <h1 class="page-title">Mis Pedidos</h1>
      <p class="page-subtitle">Rastrear y gestionar tus pedidos de impresión</p>
    </div>

    <!-- Orders List -->
    <div class="orders-container">
      <div v-if="orders.length === 0" class="empty-state">
        <span class="empty-icon">📦</span>
        <h2>No hay pedidos aún</h2>
        <p>Comienza a crear tu primer pedido</p>
        <button class="btn btn-primary" @click="$emit('navigate', 'crear-pedido')">
          Crear un Pedido
        </button>
      </div>

      <div v-for="order in orders" :key="order.id" class="order-card">
        <!-- Order Image -->
        <div class="order-image">
          <img :src="order.image" :alt="order.name">
        </div>

        <!-- Order Info -->
        <div class="order-info">
          <div class="order-header">
            <div>
              <h3 class="order-name">{{ order.name }}</h3>
              <p class="order-id">{{ order.orderNumber }}</p>
            </div>
            <span class="order-status" :class="'status-' + order.status.toLowerCase()">
              {{ order.status }}
            </span>
          </div>

          <!-- Order Details -->
          <div class="order-details">
            <div class="detail-item">
              <span class="detail-icon">📅</span>
              <span class="detail-text">{{ order.date }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-icon">📦</span>
              <span class="detail-text">Cantidad: {{ order.quantity }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-icon">💰</span>
              <span class="detail-price">${{ order.price }}</span>
            </div>
          </div>

          <!-- View Details Link -->
          <button class="view-details-btn" @click="handleViewDetails(order.id)">
            <span>👁️</span> Ver Detalles
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useRouter } from 'vue-router'

export default {
  name: 'MyOrders',
  emits: ['navigate'],
  setup() {
    const router = useRouter()
    return { router }
  },
  data() {
    return {
      orders: [
        {
          id: 1,
          name: 'Canvas Print',
          orderNumber: 'Order #ORD-001',
          date: '7/5/2026',
          quantity: 2,
          price: '89.98',
          status: 'Delivered',
          image: 'https://images.unsplash.com/photo-1549887534-7e5d8e4bedc1?w=150&h=150&fit=crop'
        },
        {
          id: 2,
          name: 'Poster',
          orderNumber: 'Order #ORD-002',
          date: '4/5/2026',
          quantity: 5,
          price: '124.95',
          status: 'Shipped',
          image: 'https://images.unsplash.com/photo-1470114716159-e389f8712fda?w=150&h=150&fit=crop'
        },
        {
          id: 3,
          name: 'Metal Print',
          orderNumber: 'Order #ORD-003',
          date: '2/5/2026',
          quantity: 1,
          price: '79.99',
          status: 'Processing',
          image: 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=150&h=150&fit=crop'
        }
      ]
    }
  },
  methods: {
    goHome() {
      this.router.push('/home')
    },
    handleViewDetails(orderId) {
      console.log('Ver detalles del pedido:', orderId);
      alert('Detalles del pedido en desarrollo');
    }
  }
}
</script>

<style scoped>
.my-orders {
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

/* Orders Container */
.orders-container {
  display: flex;
  flex-direction: column;
  gap: 20px;
  padding: 0 20px;
  max-width: 100%;
}

/* Empty State */
.empty-state {
  background: white;
  padding: 80px 40px;
  border-radius: 20px;
  text-align: center;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
}

.empty-icon {
  font-size: 3rem;
  display: block;
  margin-bottom: 20px;
}

.empty-state h2 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 10px 0;
}

.empty-state p {
  color: #666;
  margin-bottom: 30px;
}

.btn-primary {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  color: white;
  padding: 12px 30px;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
  font-family: inherit;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(160, 32, 240, 0.3);
}

/* Order Card */
.order-card {
  background: white;
  padding: 25px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
  display: grid;
  grid-template-columns: 150px 1fr;
  gap: 30px;
  align-items: center;
  transition: all 0.3s ease;
}

.order-card:hover {
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.12);
  transform: translateY(-2px);
}

/* Order Image */
.order-image {
  width: 150px;
  height: 150px;
  border-radius: 15px;
  overflow: hidden;
  background: #f5f5f5;
}

.order-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

/* Order Info */
.order-info {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.order-header div {
  flex: 1;
}

.order-name {
  font-size: 1.3rem;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0;
}

.order-id {
  color: #999;
  font-size: 0.9rem;
  margin: 5px 0 0 0;
}

/* Status Badge */
.order-status {
  padding: 6px 15px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
  white-space: nowrap;
}

.status-delivered {
  background: rgba(52, 211, 153, 0.2);
  color: #047857;
}

.status-shipped {
  background: rgba(168, 85, 247, 0.2);
  color: #7c3aed;
}

.status-processing {
  background: rgba(59, 130, 246, 0.2);
  color: #2563eb;
}

.status-pending {
  background: rgba(251, 146, 60, 0.2);
  color: #d97706;
}

.status-cancelled {
  background: rgba(239, 68, 68, 0.2);
  color: #dc2626;
}

/* Order Details */
.order-details {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 20px;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 10px;
}

.detail-icon {
  font-size: 1.2rem;
}

.detail-text {
  color: #666;
  font-size: 0.95rem;
}

.detail-price {
  color: #ff7a00;
  font-weight: 700;
  font-size: 1.1rem;
}

/* View Details Button */
.view-details-btn {
  background: none;
  border: none;
  color: #a020f0;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 0.95rem;
  transition: color 0.3s ease;
  align-self: flex-start;
}

.view-details-btn:hover {
  color: #ff006e;
  text-decoration: underline;
}

/* Responsive */
@media (max-width: 768px) {
  .my-orders {
    padding: 20px 15px;
  }

  .page-title {
    font-size: 2rem;
  }

  .order-card {
    grid-template-columns: 120px 1fr;
    gap: 20px;
    padding: 20px;
  }

  .order-image {
    width: 120px;
    height: 120px;
  }

  .order-details {
    grid-template-columns: 1fr;
    gap: 12px;
  }

  .order-header {
    flex-direction: column;
    gap: 10px;
  }

  .order-status {
    align-self: flex-start;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.5rem;
  }

  .order-card {
    grid-template-columns: 100px 1fr;
    gap: 15px;
    padding: 15px;
  }

  .order-image {
    width: 100px;
    height: 100px;
  }

  .order-name {
    font-size: 1.1rem;
  }

  .detail-item {
    gap: 8px;
  }

  .detail-icon {
    font-size: 1rem;
  }

  .detail-text,
  .detail-price {
    font-size: 0.85rem;
  }

  .empty-state {
    padding: 40px 20px;
  }
}
</style>
