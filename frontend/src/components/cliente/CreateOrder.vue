<template>
  <div class="create-order">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="goHome">
        <span>←</span> Volver al Inicio
      </button>
      <h1 class="page-title">Crea Tu Pedido</h1>
      <p class="page-subtitle">Sigue los pasos para personalizar tu impresión perfecta</p>
    </div>

    <!-- Steps Indicator -->
    <div class="steps-container">
      <div class="step" :class="{ active: currentStep === 1 }">
        <span class="step-number">1</span>
        <span class="step-label">Elige Diseño</span>
      </div>
      <div class="step" :class="{ active: currentStep === 2 }">
        <span class="step-number">2</span>
        <span class="step-label">Detalles</span>
      </div>
      <div class="step" :class="{ active: currentStep === 3 }">
        <span class="step-number">3</span>
        <span class="step-label">Revisar & Pedir</span>
      </div>
    </div>

    <!-- Content Area -->
    <div class="content-wrapper">
      <!-- Left Section -->
      <div class="left-section">
        <!-- Upload Image -->
        <div class="card">
          <div class="card-header">
            <span class="icon">📤</span>
            <h3>Subir Imagen</h3>
          </div>
          <div class="upload-area" @click="triggerFileInput" @drop.prevent="handleDrop" @dragover.prevent>
            <div class="upload-content">
              <span class="upload-icon">📁</span>
              <p class="upload-text">Haz clic para subir o arrastra y suelta</p>
              <p class="upload-info">PNG, JPG, GIF hasta 10MB</p>
            </div>
            <input 
              ref="fileInput" 
              type="file" 
              accept="image/*" 
              @change="handleFileUpload"
              style="display: none"
            >
          </div>
        </div>

        <!-- Generate with AI -->
        <div class="card">
          <div class="card-header">
            <span class="icon">✨</span>
            <h3>Generar con IA</h3>
          </div>
          <textarea 
            v-model="aiPrompt"
            placeholder="Describe tu diseño... ej., 'Una puesta de sol vibrante sobre montañas con colores púrpura y naranja'"
            class="prompt-input"
          ></textarea>
          <button class="btn btn-generate" @click="handleAIGenerate">
            Generar Imagen
          </button>
        </div>
      </div>

      <!-- Right Section -->
      <div class="right-section">
        <div class="preview-card">
          <h3>Vista Previa</h3>
          <div class="preview-container">
            <div v-if="!previewImage" class="preview-placeholder">
              <span class="preview-icon">📥</span>
              <p>Tu diseño aparecerá aquí</p>
            </div>
            <img v-else :src="previewImage" :alt="previewAlt" class="preview-image">
          </div>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <div class="footer">
      <button class="btn btn-next" @click="handleNext" :disabled="!previewImage">
        Siguiente <span>→</span>
      </button>
    </div>
  </div>
</template>

<script>
import { useRouter } from 'vue-router'

export default {
  name: 'CreateOrder',
  emits: ['navigate'],
  setup() {
    const router = useRouter()
    return { router }
  },
  data() {
    return {
      currentStep: 1,
      previewImage: null,
      previewAlt: 'preview',
      aiPrompt: '',
    }
  },
  methods: {
    goHome() {
      this.router.push('/home')
    },
    triggerFileInput() {
      this.$refs.fileInput.click();
    },
    handleFileUpload(event) {
      const file = event.target.files[0];
      if (file) {
        const reader = new FileReader();
        reader.onload = (e) => {
          this.previewImage = e.target.result;
          this.previewAlt = file.name;
        };
        reader.readAsDataURL(file);
      }
    },
    handleDrop(event) {
      const files = event.dataTransfer.files;
      if (files.length > 0) {
        const file = files[0];
        if (file.type.startsWith('image/')) {
          const reader = new FileReader();
          reader.onload = (e) => {
            this.previewImage = e.target.result;
            this.previewAlt = file.name;
          };
          reader.readAsDataURL(file);
        }
      }
    },
    handleAIGenerate() {
      if (this.aiPrompt.trim()) {
        console.log('Generando con IA:', this.aiPrompt);
        alert('Generación con IA en desarrollo');
        // Simulación de imagen generada
        this.previewImage = 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=500&h=500&fit=crop';
        this.previewAlt = 'Generated image';
      }
    },
    handleNext() {
      if (this.previewImage) {
        this.currentStep = 2;
        console.log('Ir al paso 2');
      }
    }
  }
}
</script>

<style scoped>
.create-order {
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
  color: #a020f0;
  margin-bottom: 10px;
}

.page-subtitle {
  font-size: 1rem;
  color: #666;
}

/* Steps Indicator */
.steps-container {
  display: flex;
  justify-content: center;
  gap: 40px;
  margin-bottom: 50px;
  padding: 20px;
}

.step {
  display: flex;
  align-items: center;
  gap: 12px;
  opacity: 0.5;
  transition: opacity 0.3s ease;
}

.step.active {
  opacity: 1;
}

.step-number {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.1rem;
}

.step:not(.active) .step-number {
  background: #e0e0e0;
  color: #999;
}

.step-label {
  font-weight: 600;
  color: #1a1a1a;
}

.step:not(.active) .step-label {
  color: #999;
}

/* Content Wrapper */
.content-wrapper {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 40px;
  margin-bottom: 40px;
  padding: 0 20px;
}

/* Cards */
.card {
  background: white;
  padding: 30px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
  margin-bottom: 30px;
}

.card-header {
  display: flex;
  align-items: center;
  gap: 15px;
  margin-bottom: 25px;
}

.icon {
  font-size: 1.8rem;
}

.card-header h3 {
  font-size: 1.3rem;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0;
}

/* Upload Area */
.upload-area {
  border: 2px dashed #ddd;
  border-radius: 15px;
  padding: 40px 20px;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.upload-area:hover {
  border-color: #a020f0;
  background-color: rgba(160, 32, 240, 0.05);
}

.upload-content {
  pointer-events: none;
}

.upload-icon {
  font-size: 3rem;
  display: block;
  margin-bottom: 15px;
}

.upload-text {
  font-size: 1rem;
  font-weight: 600;
  color: #1a1a1a;
  margin: 0 0 5px 0;
}

.upload-info {
  font-size: 0.85rem;
  color: #999;
  margin: 0;
}

/* Prompt Input */
.prompt-input {
  width: 100%;
  padding: 15px;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  font-family: inherit;
  font-size: 0.95rem;
  resize: vertical;
  min-height: 120px;
  transition: all 0.3s ease;
  margin-bottom: 15px;
}

.prompt-input:focus {
  outline: none;
  border-color: #a020f0;
  box-shadow: 0 0 0 3px rgba(160, 32, 240, 0.1);
}

/* Buttons */
.btn {
  padding: 12px 30px;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
  font-family: inherit;
}

.btn-generate {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  color: white;
  width: 100%;
  padding: 15px;
}

.btn-generate:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(160, 32, 240, 0.3);
}

.btn-generate:active {
  transform: translateY(0);
}

.btn-next {
  background: linear-gradient(135deg, #a020f0 0%, #ff006e 50%, #ff7a00 100%);
  color: white;
  padding: 15px 50px;
  font-size: 1.1rem;
  display: flex;
  align-items: center;
  gap: 10px;
}

.btn-next:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 15px 35px rgba(160, 32, 240, 0.3);
}

.btn-next:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Preview Section */
.preview-card {
  background: white;
  padding: 30px;
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
  height: 100%;
  display: flex;
  flex-direction: column;
}

.preview-card h3 {
  font-size: 1.3rem;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 20px 0;
}

.preview-container {
  flex: 1;
  border: 2px dashed #e0e0e0;
  border-radius: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 400px;
}

.preview-placeholder {
  text-align: center;
  color: #999;
  pointer-events: none;
}

.preview-icon {
  font-size: 3rem;
  display: block;
  margin-bottom: 10px;
}

.preview-placeholder p {
  margin: 0;
  font-size: 0.95rem;
}

.preview-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 13px;
}

/* Footer */
.footer {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  padding: 20px 0;
}

/* Responsive */
@media (max-width: 1024px) {
  .content-wrapper {
    grid-template-columns: 1fr;
    gap: 30px;
  }

  .steps-container {
    gap: 20px;
  }
}

@media (max-width: 768px) {
  .create-order {
    padding: 20px 15px;
  }

  .page-title {
    font-size: 2rem;
  }

  .steps-container {
    flex-wrap: wrap;
    gap: 15px;
  }

  .card {
    padding: 20px;
    margin-bottom: 20px;
  }

  .preview-container {
    min-height: 300px;
  }

  .footer {
    justify-content: center;
  }

  .btn-next {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.5rem;
  }

  .steps-container {
    flex-direction: column;
    gap: 10px;
  }

  .step {
    gap: 8px;
  }

  .step-number {
    width: 35px;
    height: 35px;
    font-size: 0.9rem;
  }

  .card {
    padding: 15px;
  }

  .upload-area {
    padding: 25px 15px;
  }

  .upload-icon {
    font-size: 2rem;
  }

  .preview-container {
    min-height: 250px;
  }
}
</style>
