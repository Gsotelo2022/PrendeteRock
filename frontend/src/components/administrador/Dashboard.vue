<template>
  <div class="admin-dashboard">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="$emit('navigate', 'home')">
        <span>←</span> Volver al Inicio
      </button>
      <h1 class="page-title">Panel de Administración</h1>
      <p class="page-subtitle">Resumen de tu negocio de impresión</p>
    </div>

    <!-- Stats Cards -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Cargando métricas...</p>
    </div>

    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="retry-btn" @click="loadMetrics">Reintentar</button>
    </div>

    <div v-else class="stats-grid">
      <div class="stat-card">
        <div class="stat-header">
          <span class="stat-icon revenue">💵</span>
        </div>
        <p class="stat-label">Ingresos Totales</p>
        <p class="stat-value">${{ metrics.totalRevenue.toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }}</p>
      </div>

      <div class="stat-card">
        <div class="stat-header">
          <span class="stat-icon orders">🛒</span>
        </div>
        <p class="stat-label">Pedidos Totales</p>
        <p class="stat-value">{{ metrics.totalOrders }}</p>
      </div>

      <div class="stat-card">
        <div class="stat-header">
          <span class="stat-icon customers">👥</span>
        </div>
        <p class="stat-label">Clientes Totales</p>
        <p class="stat-value">{{ metrics.totalCustomers }}</p>
      </div>

      <div class="stat-card">
        <div class="stat-header">
          <span class="stat-icon products">📦</span>
        </div>
        <p class="stat-label">Productos Activos</p>
        <p class="stat-value">{{ metrics.totalActiveProducts }}</p>
      </div>
    </div>

    <!-- Recent Orders Table -->
    <div v-if="!loading && !error && metrics.recentOrders.length > 0" class="recent-orders">
      <h3>Pedidos Recientes</h3>
      <div class="table-wrapper">
        <table class="orders-table">
          <thead>
            <tr>
              <th>ID de Pedido</th>
              <th>Cliente</th>
              <th>Producto</th>
              <th>Monto</th>
              <th>Estado</th>
              <th>Fecha</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="order in metrics.recentOrders" :key="order.id">
              <td class="order-id-cell">{{ order.orderId }}</td>
              <td>{{ order.customer }}</td>
              <td class="product-cell">{{ order.product }}</td>
              <td class="amount-cell">${{ order.amount.toLocaleString('es-AR', { minimumFractionDigits: 2 }) }}</td>
              <td>
                <span class="status-badge" :class="'status-' + order.status.toLowerCase()">
                  {{ translateStatus(order.status) }}
                </span>
              </td>
              <td class="date-cell">{{ formatDate(order.createdAt) }}</td>
            </tr>
          </tbody>
        </table>        
      </div>
    </div>

    <!-- Charts Section -->
    <div v-if="!loading && !error && chartData.length > 0" class="charts-grid">
      <!-- Revenue Trend Chart -->
      <div class="chart-card">
        <h3>Tendencia de Ingresos (Últimos 6 Meses)</h3>
        <div class="chart">
          <div class="chart-placeholder">
            <svg viewBox="0 0 400 250" preserveAspectRatio="xMidYMid meet">
              <!-- Y Axis -->
              <line x1="50" y1="30" x2="50" y2="220" stroke="#e0e0e0" stroke-width="2"/>
              <!-- X Axis -->
              <line x1="50" y1="220" x2="390" y2="220" stroke="#e0e0e0" stroke-width="2"/>
              
              <!-- Y Axis Labels -->
              <text x="10" y="230" font-size="12" fill="#999">0</text>
              <text x="10" y="160" font-size="12" fill="#999">{{ formatCurrency(maxRevenue * 0.33) }}</text>
              <text x="10" y="90" font-size="12" fill="#999">{{ formatCurrency(maxRevenue * 0.66) }}</text>
              <text x="10" y="30" font-size="12" fill="#999">{{ formatCurrency(maxRevenue) }}</text>
              
              <!-- X Axis Labels -->
              <text v-for="(item, index) in chartData" :key="'label-' + index" 
                    :x="70 + (index * 65)" y="245" font-size="12" fill="#999">
                {{ item.monthLabel }}
              </text>
              
              <!-- Grid Lines -->
              <line x1="50" y1="150" x2="390" y2="150" stroke="#f0f0f0" stroke-width="1"/>
              <line x1="50" y1="80" x2="390" y2="80" stroke="#f0f0f0" stroke-width="1"/>
              
              <!-- Line Chart -->
              <polyline :points="revenueLinePoints" 
                        stroke="url(#gradientLine)" stroke-width="3" fill="none" stroke-linecap="round"/>
              
              <!-- Points -->
              <circle v-for="(item, index) in chartData" :key="'point-' + index"
                      :cx="70 + (index * 65)" 
                      :cy="getRevenueY(item.revenue)" 
                      r="5" fill="#a020f0"/>
              
              <!-- Gradient Definition -->
              <defs>
                <linearGradient id="gradientLine" x1="0%" y1="0%" x2="100%" y2="0%">
                  <stop offset="0%" style="stop-color:#a020f0;stop-opacity:1" />
                  <stop offset="50%" style="stop-color:#ff006e;stop-opacity:1" />
                  <stop offset="100%" style="stop-color:#ff7a00;stop-opacity:1" />
                </linearGradient>
              </defs>
            </svg>
          </div>
        </div>
      </div>
    
      <!-- Orders by Month Chart -->
      <div class="chart-card">
        <h3>Pedidos por Mes (Últimos 6 Meses)</h3>
        <div class="chart">
          <div class="bar-chart">
            <div v-for="(item, index) in chartData" :key="'bar-' + index" class="bar-item">
              <div class="bar" :style="getBarStyle(item.orders)"></div>
              <p class="bar-label">{{ item.monthLabel }}</p>
              <p class="bar-count">{{ item.orders }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- No data message -->
    <div v-if="!loading && !error && chartData.length === 0" class="no-data-message">
      <p>📊 No hay datos suficientes para mostrar los gráficos</p>
    </div>
  </div>
</template>

<script>
import { useApi } from '../../composables/useApi'

export default {
  name: 'AdminDashboard',
  setup() {
    const api = useApi()
    return { api }
  },
  data() {
    return {
      loading: true,
      error: null,
      metrics: {
        totalRevenue: 0,
        totalOrders: 0,
        totalCustomers: 0,
        totalActiveProducts: 0,
        recentOrders: [],
        revenueByMonth: [],
        ordersByMonth: []
      }
    }
  },
  computed: {
    chartData() {
      // Procesar datos para los últimos 6 meses
      const months = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'];
      const result = [];
      
      // Crear un mapa de los últimos 6 meses
      for (let i = 5; i >= 0; i--) {
        const date = new Date();
        date.setMonth(date.getMonth() - i);
        const year = date.getFullYear();
        const month = date.getMonth() + 1;
        
        const revenueData = this.metrics.revenueByMonth?.find(r => r.year === year && r.month === month);
        const ordersData = this.metrics.ordersByMonth?.find(o => o.year === year && o.month === month);
        
        result.push({
          monthLabel: months[month - 1],
          revenue: revenueData?.revenue || 0,
          orders: ordersData?.count || 0
        });
      }
      
      return result;
    },
    maxRevenue() {
      if (this.chartData.length === 0) return 1000;
      const max = Math.max(...this.chartData.map(d => d.revenue));
      return max > 0 ? Math.ceil(max / 1000) * 1000 : 1000;
    },
    maxOrders() {
      if (this.chartData.length === 0) return 10;
      const max = Math.max(...this.chartData.map(d => d.orders));
      return max > 0 ? max : 10;
    },
    revenueLinePoints() {
      return this.chartData.map((item, index) => {
        const x = 70 + (index * 65);
        const y = this.getRevenueY(item.revenue);
        return `${x},${y}`;
      }).join(' ');
    }
  },
  mounted() {
    this.loadMetrics()
  },
  methods: {
    async loadMetrics() {
      try {
        this.loading = true
        this.error = null
        const data = await this.api.get('/dashboard/metrics')
        this.metrics = data
      } catch (err) {
        console.error('Error cargando métricas:', err)
        this.error = 'Error al cargar las métricas. Por favor, intenta nuevamente.'
      } finally {
        this.loading = false
      }
    },
    translateStatus(status) {
      const statusMap = {
        'Pending': 'Pendiente',
        'Processing': 'Procesando',
        'Shipped': 'Enviado',
        'Delivered': 'Entregado',
        'Cancelled': 'Cancelado'
      }
      return statusMap[status] || status
    },
    formatDate(date) {
      return new Date(date).toLocaleDateString('es-AR', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
      })
    },
    getRevenueY(revenue) {
      // Mapear el revenue al eje Y (220 = bottom, 30 = top)
      const percentage = this.maxRevenue > 0 ? revenue / this.maxRevenue : 0;
      return 220 - (percentage * 190);
    },
    getBarStyle(orderCount) {
      // Calcular altura de la barra basada en el máximo
      const percentage = this.maxOrders > 0 ? (orderCount / this.maxOrders) * 100 : 0;
      const height = Math.max(percentage, 5); // Mínimo 5% para visibilidad
      return {
        height: `${height}%`,
        background: 'linear-gradient(180deg, #a020f0 0%, #ff006e 100%)'
      };
    },
    formatCurrency(value) {
      if (value >= 1000) {
        return `$${(value / 1000).toFixed(1)}k`;
      }
      return `$${Math.round(value)}`;
    }
  }
}
</script>

<style scoped>
.admin-dashboard {
  padding: 40px 0;
  background-color: #faf7f2;
  min-height: 100vh;
}

/* Header */
.header {
  margin-bottom: 40px;
  padding: 0 20px;
}

/* Loading and Error States */
.loading-state,
.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  background: white;
  border-radius: 20px;
  margin: 0 20px 40px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
}

.loading-state p,
.error-state p {
  margin-top: 20px;
  color: #666;
  font-size: 1rem;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #a020f0;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.retry-btn {
  margin-top: 15px;
  padding: 10px 24px;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s ease;
}

.retry-btn:hover {
  transform: translateY(-2px);
}

.no-data-message {
  text-align: center;
  padding: 40px 20px;
  margin: 0 20px 40px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
}

.no-data-message p {
  font-size: 1.1rem;
  color: #666;
  margin: 0;
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

/* Stats Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 40px;
  padding: 0 20px;
}

.stat-card {
  background: white;
  padding: 25px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.12);
}

.stat-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.stat-icon {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.8rem;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
}

.stat-change {
  padding: 5px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.stat-change.positive {
  background: rgba(16, 185, 129, 0.2);
  color: #057a55;
}

.stat-label {
  color: #666;
  font-size: 0.9rem;
  margin: 0 0 10px 0;
}

.stat-value {
  font-size: 1.8rem;
  font-weight: 800;
  color: #1a1a1a;
  margin: 0;
}

/* Charts Grid */
.charts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 25px;
  margin-bottom: 40px;
  padding: 0 20px;
}

.chart-card {
  background: white;
  padding: 30px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
}

.chart-card h3 {
  font-size: 1.2rem;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 25px 0;
}

.chart {
  min-height: 250px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.chart-placeholder svg {
  width: 100%;
  height: 100%;
}

/* Bar Chart */
.bar-chart {
  display: flex;
  align-items: flex-end;
  justify-content: space-around;
  height: 200px;
  gap: 15px;
}

.bar-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  flex: 1;
}

.bar {
  width: 100%;
  border-radius: 8px 8px 0 0;
  transition: all 0.3s ease;
  min-height: 30px;
}

.bar:hover {
  opacity: 0.8;
  transform: scaleY(1.05);
}

.bar-label {
  font-size: 0.85rem;
  color: #666;
  margin: 0;
}

.bar-count {
  font-size: 0.75rem;
  color: #a020f0;
  font-weight: 600;
  margin: 5px 0 0 0;
}

/* Recent Orders */
.recent-orders {
  background: white;
  padding: 30px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
  margin: 0 20px 40px;
}

.recent-orders h3 {
  font-size: 1.2rem;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 25px 0;
}

.table-wrapper {
  overflow-x: auto;
}

.orders-table {
  width: 100%;
  border-collapse: collapse;
}

.orders-table thead {
  background-color: #f5f5f5;
}

.orders-table th {
  padding: 15px;
  text-align: left;
  font-weight: 600;
  color: #666;
  font-size: 0.9rem;
  border-bottom: 2px solid #e0e0e0;
}

.orders-table td {
  padding: 15px;
  border-bottom: 1px solid #e0e0e0;
  font-size: 0.95rem;
}

.orders-table tbody tr {
  transition: background-color 0.3s ease;
}

.orders-table tbody tr:hover {
  background-color: rgba(160, 32, 240, 0.05);
}

.order-id-cell {
  font-weight: 600;
  color: #a020f0;
}

.product-cell {
  color: #0066ff;
}

.amount-cell {
  font-weight: 600;
  color: #ff7a00;
}

.date-cell {
  color: #666;
  font-size: 0.9rem;
}

/* Status Badge */
.status-badge {
  display: inline-block;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
  white-space: nowrap;
}

.status-delivered {
  background: rgba(16, 185, 129, 0.2);
  color: #057a55;
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

/* Responsive */
@media (max-width: 1024px) {
  .stats-grid {
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 15px;
  }

  .charts-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .admin-dashboard {
    padding: 20px 15px;
  }

  .page-title {
    font-size: 2rem;
  }

  .stats-grid {
    grid-template-columns: 1fr 1fr;
    gap: 15px;
  }

  .stat-card {
    padding: 20px;
  }

  .stat-icon {
    width: 45px;
    height: 45px;
    font-size: 1.5rem;
  }

  .stat-value {
    font-size: 1.5rem;
  }

  .recent-orders {
    padding: 20px;
  }

  .orders-table th,
  .orders-table td {
    padding: 12px;
    font-size: 0.85rem;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.5rem;
  }

  .stats-grid {
    grid-template-columns: 1fr;
    gap: 12px;
  }

  .stat-card {
    padding: 15px;
  }

  .stat-value {
    font-size: 1.3rem;
  }

  .bar-chart {
    height: 150px;
    gap: 8px;
  }

  .orders-table th,
  .orders-table td {
    padding: 8px;
    font-size: 0.75rem;
  }

  .status-badge {
    padding: 4px 8px;
    font-size: 0.75rem;
  }
}
</style>
