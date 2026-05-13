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
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-header">
          <span class="stat-icon revenue">💵</span>
          <span class="stat-change positive">↑ +12.5%</span>
        </div>
        <p class="stat-label">Ingresos Totales</p>
        <p class="stat-value">$27,900</p>
      </div>

      <div class="stat-card">
        <div class="stat-header">
          <span class="stat-icon orders">🛒</span>
          <span class="stat-change positive">↑ +8.2%</span>
        </div>
        <p class="stat-label">Pedidos Totales</p>
        <p class="stat-value">279</p>
      </div>

      <div class="stat-card">
        <div class="stat-header">
          <span class="stat-icon customers">👥</span>
          <span class="stat-change positive">↑ +15.3%</span>
        </div>
        <p class="stat-label">Clientes Totales</p>
        <p class="stat-value">156</p>
      </div>

      <div class="stat-card">
        <div class="stat-header">
          <span class="stat-icon products">📦</span>
          <span class="stat-change positive">↑ +4.0%</span>
        </div>
        <p class="stat-label">Productos Activos</p>
        <p class="stat-value">24</p>
      </div>
    </div>

    <!-- Charts Section -->
    <div class="charts-grid">
      <!-- Revenue Trend Chart -->
      <div class="chart-card">
        <h3>Tendencia de Ingresos</h3>
        <div class="chart">
          <div class="chart-placeholder">
            <svg viewBox="0 0 400 250" preserveAspectRatio="xMidYMid meet">
              <!-- Y Axis -->
              <line x1="50" y1="30" x2="50" y2="220" stroke="#e0e0e0" stroke-width="2"/>
              <!-- X Axis -->
              <line x1="50" y1="220" x2="390" y2="220" stroke="#e0e0e0" stroke-width="2"/>
              
              <!-- Y Axis Labels -->
              <text x="20" y="230" font-size="12" fill="#999">0</text>
              <text x="15" y="160" font-size="12" fill="#999">2000</text>
              <text x="15" y="90" font-size="12" fill="#999">4000</text>
              <text x="15" y="20" font-size="12" fill="#999">8000</text>
              
              <!-- X Axis Labels -->
              <text x="45" y="245" font-size="12" fill="#999">Ene</text>
              <text x="120" y="245" font-size="12" fill="#999">Feb</text>
              <text x="185" y="245" font-size="12" fill="#999">Mar</text>
              <text x="260" y="245" font-size="12" fill="#999">Abr</text>
              <text x="340" y="245" font-size="12" fill="#999">May</text>
              
              <!-- Grid Lines -->
              <line x1="45" y1="150" x2="390" y2="150" stroke="#f0f0f0" stroke-width="1"/>
              <line x1="45" y1="80" x2="390" y2="80" stroke="#f0f0f0" stroke-width="1"/>
              
              <!-- Line Chart -->
              <polyline points="70,140 145,110 220,130 295,80 370,50" 
                        stroke="url(#gradientLine)" stroke-width="3" fill="none" stroke-linecap="round"/>
              
              <!-- Points -->
              <circle cx="70" cy="140" r="5" fill="#a020f0"/>
              <circle cx="145" cy="110" r="5" fill="#a020f0"/>
              <circle cx="220" cy="130" r="5" fill="#a020f0"/>
              <circle cx="295" cy="80" r="5" fill="#a020f0"/>
              <circle cx="370" cy="50" r="5" fill="#a020f0"/>
              
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
        <h3>Pedidos por Mes</h3>
        <div class="chart">
          <div class="bar-chart">
            <div class="bar-item">
              <div class="bar" style="height: 50%; background: linear-gradient(180deg, #a020f0 0%, #ff006e 100%);"></div>
              <p class="bar-label">Ene</p>
            </div>
            <div class="bar-item">
              <div class="bar" style="height: 60%; background: linear-gradient(180deg, #a020f0 0%, #ff006e 100%);"></div>
              <p class="bar-label">Feb</p>
            </div>
            <div class="bar-item">
              <div class="bar" style="height: 55%; background: linear-gradient(180deg, #a020f0 0%, #ff006e 100%);"></div>
              <p class="bar-label">Mar</p>
            </div>
            <div class="bar-item">
              <div class="bar" style="height: 75%; background: linear-gradient(180deg, #a020f0 0%, #ff006e 100%);"></div>
              <p class="bar-label">Abr</p>
            </div>
            <div class="bar-item">
              <div class="bar" style="height: 90%; background: linear-gradient(180deg, #a020f0 0%, #ff006e 100%);"></div>
              <p class="bar-label">May</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Orders Table -->
    <div class="recent-orders">
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
            </tr>
          </thead>
          <tbody>
            <tr v-for="order in recentOrders" :key="order.id">
              <td class="order-id-cell">{{ order.orderId }}</td>
              <td>{{ order.customer }}</td>
              <td class="product-cell">{{ order.product }}</td>
              <td class="amount-cell">${{ order.amount }}</td>
              <td>
                <span class="status-badge" :class="'status-' + order.status.toLowerCase()">
                  {{ order.status }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AdminDashboard',
  data() {
    return {
      recentOrders: [
        {
          id: 1,
          orderId: 'ORD-279',
          customer: 'John Doe',
          product: 'Canvas Print',
          amount: '44.99',
          status: 'Delivered'
        },
        {
          id: 2,
          orderId: 'ORD-278',
          customer: 'Jane Smith',
          product: 'Poster',
          amount: '24.99',
          status: 'Shipped'
        },
        {
          id: 3,
          orderId: 'ORD-277',
          customer: 'Bob Johnson',
          product: 'Metal Print',
          amount: '79.99',
          status: 'Processing'
        },
        {
          id: 4,
          orderId: 'ORD-276',
          customer: 'Alice Brown',
          product: 'Acrylic Print',
          amount: '99.99',
          status: 'Pending'
        }
      ]
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

/* Recent Orders */
.recent-orders {
  background: white;
  padding: 30px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
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
