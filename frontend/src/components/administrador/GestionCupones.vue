<template>
  <div class="gestion-cupones">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="goHome">
        <span>←</span> Volver al Inicio
      </button>
      <div class="header-top">
        <div>
          <h1 class="page-title">Coupons Management</h1>
          <p class="page-subtitle">Create and manage promotional coupons</p>
        </div>
        <button class="create-coupon-btn" @click="handleCreateCoupon">
          <span>+</span> Create Coupon
        </button>
      </div>
    </div>

    <!-- Coupons List -->
    <div class="coupons-list">
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
              <span class="detail-label">% Discount</span>
              <span class="detail-value">{{ coupon.discount }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">📅 Expires</span>
              <span class="detail-value">{{ coupon.expiryDate }}</span>
            </div>
          </div>

          <div class="detail-row">
            <div class="detail-item">
              <span class="detail-label">🎯 Usage</span>
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
          <button 
            v-if="coupon.status === 'Active'"
            class="action-btn deactivate-btn" 
            @click="handleToggleStatus(coupon.id)"
          >
            Deactivate
          </button>
          <button 
            v-else
            class="action-btn activate-btn" 
            @click="handleToggleStatus(coupon.id)"
          >
            Activate
          </button>
          <button class="action-btn delete-btn" @click="handleDelete(coupon.id)">
            🗑 Delete
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useRouter } from 'vue-router'

export default {
  name: 'GestionCupones',
  setup() {
    const router = useRouter()
    return { router }
  },
  data() {
    return {
      coupons: [
        {
          id: 1,
          code: 'SPRING2026',
          codeBg: 'linear-gradient(135deg, #a020f0 0%, #ff006e 100%)',
          status: 'Active',
          discount: '20%',
          expiryDate: '29/6/2026',
          usageCount: 45,
          usageLimit: 100,
          progressPercent: 45
        },
        {
          id: 2,
          code: 'NEWCUSTOMER',
          codeBg: 'linear-gradient(135deg, #ff006e 0%, #ff7a00 100%)',
          status: 'Active',
          discount: '$10',
          expiryDate: '30/12/2026',
          usageCount: 128,
          usageLimit: 500,
          progressPercent: 25.6
        },
        {
          id: 3,
          code: 'FLASH50',
          codeBg: 'linear-gradient(135deg, #a020f0 0%, #ff7a00 100%)',
          status: 'Inactive',
          discount: '50%',
          expiryDate: '14/5/2026',
          usageCount: 89,
          usageLimit: 100,
          progressPercent: 89
        },
        {
          id: 4,
          code: 'SAVE15',
          codeBg: 'linear-gradient(135deg, #ff7a00 0%, #a020f0 100%)',
          status: 'Active',
          discount: '15%',
          expiryDate: '30/8/2026',
          usageCount: 12,
          usageLimit: 200,
          progressPercent: 6
        }
      ]
    }
  },
  methods: {
    goHome() {
      this.router.push('/admin')
    },
    handleCreateCoupon() {
      console.log('Crear nuevo cupón')
      // TODO: Implementar modal para crear cupón
    },
    handleCopyCoupon(code) {
      navigator.clipboard.writeText(code)
      alert('Código de cupón copiado: ' + code)
    },
    handleToggleStatus(couponId) {
      const coupon = this.coupons.find(c => c.id === couponId)
      if (coupon) {
        coupon.status = coupon.status === 'Active' ? 'Inactive' : 'Active'
      }
    },
    handleDelete(couponId) {
      if (confirm('¿Estás seguro de que deseas eliminar este cupón?')) {
        this.coupons = this.coupons.filter(c => c.id !== couponId)
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
