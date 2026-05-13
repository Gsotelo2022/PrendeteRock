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
      <div v-if="filteredCustomers.length === 0" class="empty-state">
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
          <button class="action-btn edit-btn" @click="handleEdit(customer.id)">Editar</button>
          <button class="action-btn delete-btn" @click="handleDelete(customer.id)">Eliminar</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useRouter } from 'vue-router'

export default {
  name: 'GestionClientes',
  setup() {
    const router = useRouter()
    return { router }
  },
  data() {
    return {
      searchQuery: '',
      customers: [
        {
          id: 1,
          name: 'John Doe',
          customerId: 'CUST-001',
          email: 'john@example.com',
          phone: '+1 234-567-8901',
          initials: 'JD',
          avatarBg: 'linear-gradient(135deg, #a020f0 0%, #ff006e 100%)',
          totalOrders: 12,
          totalSpent: 590,
          joinDate: 'Dic 2025'
        },
        {
          id: 2,
          name: 'Jane Smith',
          customerId: 'CUST-002',
          email: 'jane@example.com',
          phone: '+1 234-567-8902',
          initials: 'JS',
          avatarBg: 'linear-gradient(135deg, #ff006e 0%, #ff7a00 100%)',
          totalOrders: 8,
          totalSpent: 425,
          joinDate: 'Ene 2026'
        },
        {
          id: 3,
          name: 'Bob Johnson',
          customerId: 'CUST-003',
          email: 'bob@example.com',
          phone: '+1 234-567-8903',
          initials: 'BJ',
          avatarBg: 'linear-gradient(135deg, #ff7a00 0%, #a020f0 100%)',
          totalOrders: 15,
          totalSpent: 880,
          joinDate: 'Nov 2025'
        },
        {
          id: 4,
          name: 'Alice Brown',
          customerId: 'CUST-004',
          email: 'alice@example.com',
          phone: '+1 234-567-8904',
          initials: 'AB',
          avatarBg: 'linear-gradient(135deg, #a020f0 0%, #ff7a00 100%)',
          totalOrders: 5,
          totalSpent: 300,
          joinDate: 'Feb 2026'
        },
        {
          id: 5,
          name: 'Charlie Wilson',
          customerId: 'CUST-005',
          email: 'charlie@example.com',
          phone: '+1 234-567-8905',
          initials: 'CW',
          avatarBg: 'linear-gradient(135deg, #ff006e 0%, #ff7a00 100%)',
          totalOrders: 20,
          totalSpent: 1250,
          joinDate: 'Sep 2025'
        }
      ]
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
  methods: {
    goHome() {
      this.router.push('/admin')
    },
    handleViewDetails(customerId) {
      console.log('Ver detalles del cliente:', customerId)
      // TODO: Implementar vista de detalles
    },
    handleEdit(customerId) {
      console.log('Editar cliente:', customerId)
      // TODO: Implementar edición de cliente
    },
    handleDelete(customerId) {
      if (confirm('¿Estás seguro de que deseas eliminar este cliente?')) {
        console.log('Eliminar cliente:', customerId)
        // TODO: Implementar eliminación de cliente
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
}
</style>
