<template>
  <div class="my-designs">
    <!-- Header -->
    <div class="header">
        <button class="back-btn" @click="goHome">
            <span>←</span> Volver al Inicio
        </button>
        <h1 class="page-title">Mis Diseños</h1>
        <p class="page-subtitle">Explora y gestiona tus diseños guardados</p>
    </div>

    <!-- Designs Grid -->
    <div class="designs-container">
      <div v-if="designs.length === 0" class="empty-state">
        <span class="empty-icon">🎨</span>
        <h2>No hay diseños aún</h2>
        <p>Crea tu primer diseño personalizado</p>
        <button class="btn btn-primary" @click="$emit('navigate', 'crear-pedido')">
          Crear Diseño
        </button>
      </div>

      <div v-else class="designs-grid">
        <div v-for="design in designs" :key="design.id" class="design-card">
          <!-- Design Image -->
          <div class="design-image-wrapper">
            <img :src="design.image" :alt="design.name" class="design-image">
            <span class="design-badge" :class="'badge-' + design.type.toLowerCase()">
              {{ design.type }}
            </span>
          </div>

          <!-- Design Info -->
          <div class="design-info">
            <h3 class="design-name">{{ design.name }}</h3>
            <p class="design-date">Creado {{ design.date }}</p>
          </div>

          <!-- Design Actions -->
          <div class="design-actions">
            <button class="btn btn-order" @click="handleOrder(design.id)">
              <span>📦</span> Pedir
            </button>
            <button class="btn btn-icon" @click="handleDownload(design.id)" title="Descargar">
              <span>⬇️</span>
            </button>
            <button class="btn btn-icon btn-delete" @click="handleDelete(design.id)" title="Eliminar">
              <span>🗑️</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useRouter } from 'vue-router'

export default {
  name: 'MyDesigns',
  emits: ['navigate'],
  setup() {
    const router = useRouter()
    return { router }
  },
  data() {
    return {
      designs: [
        {
          id: 1,
          name: 'Mountain Sunset',
          date: '7/5/2026',
          type: 'AI Generated',
          image: 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=300&h=300&fit=crop'
        },
        {
          id: 2,
          name: 'Abstract Art',
          date: '6/5/2026',
          type: 'Uploaded',
          image: 'https://images.unsplash.com/photo-1579783902614-e3fb5141b0cb?w=300&h=300&fit=crop'
        },
        {
          id: 3,
          name: 'Ocean Waves',
          date: '5/5/2026',
          type: 'AI Generated',
          image: 'https://images.unsplash.com/photo-1505142468610-359e7d316be0?w=300&h=300&fit=crop'
        },
        {
          id: 4,
          name: 'City Skyline',
          date: '4/5/2026',
          type: 'Uploaded',
          image: 'https://images.unsplash.com/photo-1449824913935-59a10b8d2000?w=300&h=300&fit=crop'
        }
      ]
    }
  },
  methods: {
    goHome() {
      this.router.push('/home')
    },
    handleOrder(designId) {
      console.log('Pedir diseño:', designId);
      this.$emit('navigate', 'crear-pedido');
    },
    handleDownload(designId) {
      console.log('Descargar diseño:', designId);
      alert('Descarga en desarrollo');
    },
    handleDelete(designId) {
      if (confirm('¿Estás seguro de que deseas eliminar este diseño?')) {
        this.designs = this.designs.filter(d => d.id !== designId);
        console.log('Diseño eliminado:', designId);
      }
    }
  }
}
</script>

<style scoped>
.my-designs {
  padding: 40px 0;
  background-color: #faf7f2;
  min-height: 100vh;
}

/* Header */
.header {
  margin-bottom: 50px;
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

/* Empty State */
.empty-state {
  background: white;
  padding: 80px 40px;
  border-radius: 20px;
  text-align: center;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
}

.empty-icon {
  font-size: 3.5rem;
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

/* Designs Grid */
.designs-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 25px;
  padding: 0 20px;
}

/* Design Card */
.design-card {
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
}

.design-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.12);
}

/* Design Image */
.design-image-wrapper {
  position: relative;
  width: 100%;
  height: 250px;
  overflow: hidden;
  background: #f5f5f5;
}

.design-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.design-card:hover .design-image {
  transform: scale(1.05);
}

/* Design Badge */
.design-badge {
  position: absolute;
  top: 15px;
  right: 15px;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
  color: white;
  background: linear-gradient(135deg, #a020f0 0%, #c946ff 100%);
}

.badge-uploaded {
  background: linear-gradient(135deg, #0066ff 0%, #3399ff 100%);
}

.badge-ai {
  background: linear-gradient(135deg, #a020f0 0%, #c946ff 100%);
}

/* Design Info */
.design-info {
  padding: 20px 20px 0 20px;
  flex: 1;
}

.design-name {
  font-size: 1.1rem;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 5px 0;
}

.design-date {
  color: #999;
  font-size: 0.9rem;
  margin: 0;
}

/* Design Actions */
.design-actions {
  padding: 15px 20px 20px 20px;
  display: flex;
  gap: 10px;
  align-items: center;
}

.btn {
  padding: 10px 15px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  font-size: 0.9rem;
  transition: all 0.3s ease;
  font-family: inherit;
  display: flex;
  align-items: center;
  gap: 5px;
}

.btn-order {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  color: white;
  flex: 1;
  justify-content: center;
}

.btn-order:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(160, 32, 240, 0.3);
}

.btn-icon {
  background: #f0f0f0;
  color: #1a1a1a;
  width: 40px;
  height: 40px;
  padding: 0;
  justify-content: center;
  border-radius: 8px;
}

.btn-icon:hover {
  background: #e0e0e0;
}

.btn-delete:hover {
  background: rgba(239, 68, 68, 0.2);
  color: #ef4444;
}

/* Responsive */
@media (max-width: 1024px) {
  .designs-grid {
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 20px;
  }
}

@media (max-width: 768px) {
  .my-designs {
    padding: 20px 15px;
  }

  .page-title {
    font-size: 2rem;
  }

  .designs-grid {
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 15px;
  }

  .design-image-wrapper {
    height: 200px;
  }

  .empty-state {
    padding: 40px 20px;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.5rem;
  }

  .designs-grid {
    grid-template-columns: 1fr;
    gap: 15px;
  }

  .design-image-wrapper {
    height: 220px;
  }

  .design-actions {
    flex-wrap: wrap;
  }

  .btn-order {
    flex: 1 1 100%;
  }

  .btn-icon {
    flex: 1 1 calc(50% - 5px);
  }
}
</style>
