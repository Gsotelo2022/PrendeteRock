<template>
  <div class="toast-container">
    <transition-group name="toast">
      <div 
        v-for="toast in toasts" 
        :key="toast.id" 
        :class="['toast', `toast-${toast.type}`]"
        @click="removeToast(toast.id)"
      >
        <div class="toast-icon">
          <span v-if="toast.type === 'success'">✓</span>
          <span v-else-if="toast.type === 'error'">✕</span>
          <span v-else-if="toast.type === 'warning'">⚠</span>
          <span v-else>ℹ</span>
        </div>
        <div class="toast-message">{{ toast.message }}</div>
        <button class="toast-close" @click.stop="removeToast(toast.id)">✕</button>
      </div>
    </transition-group>
  </div>
</template>

<script>
import { useToast } from '../composables/useToast'

export default {
  name: 'ToastContainer',
  setup() {
    const { toasts, removeToast } = useToast()
    return { toasts, removeToast }
  }
}
</script>

<style scoped>
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 10px;
  max-width: 400px;
  pointer-events: none;
}

.toast {
  background: white;
  padding: 16px 20px;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  display: flex;
  align-items: center;
  gap: 12px;
  min-width: 300px;
  pointer-events: all;
  cursor: pointer;
  transition: all 0.3s ease;
  border-left: 4px solid;
}

.toast:hover {
  transform: translateX(-5px);
  box-shadow: 0 6px 25px rgba(0, 0, 0, 0.2);
}

.toast-icon {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 14px;
  flex-shrink: 0;
}

.toast-message {
  flex: 1;
  font-size: 0.95rem;
  color: #333;
  font-weight: 500;
}

.toast-close {
  background: none;
  border: none;
  color: #999;
  font-size: 1.2rem;
  cursor: pointer;
  padding: 0;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: color 0.2s ease;
  flex-shrink: 0;
}

.toast-close:hover {
  color: #333;
}

/* Success Toast */
.toast-success {
  border-left-color: #10b981;
}

.toast-success .toast-icon {
  background: #10b981;
  color: white;
}

/* Error Toast */
.toast-error {
  border-left-color: #ff006e;
}

.toast-error .toast-icon {
  background: #ff006e;
  color: white;
}

/* Warning Toast */
.toast-warning {
  border-left-color: #ff7a00;
}

.toast-warning .toast-icon {
  background: #ff7a00;
  color: white;
}

/* Info Toast */
.toast-info {
  border-left-color: #a020f0;
}

.toast-info .toast-icon {
  background: #a020f0;
  color: white;
}

/* Animations */
.toast-enter-active {
  animation: toast-in 0.3s ease;
}

.toast-leave-active {
  animation: toast-out 0.3s ease;
}

@keyframes toast-in {
  from {
    opacity: 0;
    transform: translateX(100px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes toast-out {
  from {
    opacity: 1;
    transform: translateX(0);
  }
  to {
    opacity: 0;
    transform: translateX(100px);
  }
}

/* Responsive */
@media (max-width: 768px) {
  .toast-container {
    top: 10px;
    right: 10px;
    left: 10px;
    max-width: none;
  }

  .toast {
    min-width: auto;
  }
}
</style>
