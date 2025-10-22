<template>
  <Transition name="toast-fade">
    <div v-if="visible" class="toast" :class="type">
      <i :class="iconClass"></i>
      <span>{{ message }}</span>
    </div>
  </Transition>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  message: {
    type: String,
    default: ''
  },
  type: {
    type: String,
    default: 'success' // success, info, warning, error
  },
  duration: {
    type: Number,
    default: 3000
  }
})

const visible = ref(false)
let timer = null

const iconClass = computed(() => {
  const icons = {
    success: 'fas fa-check-circle',
    info: 'fas fa-info-circle',
    warning: 'fas fa-exclamation-circle',
    error: 'fas fa-times-circle'
  }
  return icons[props.type] || icons.info
})

const show = () => {
  visible.value = true
  if (timer) clearTimeout(timer)
  if (props.duration > 0) {
    timer = setTimeout(() => {
      visible.value = false
    }, props.duration)
  }
}

const hide = () => {
  visible.value = false
}

watch(() => props.message, (newVal) => {
  if (newVal) {
    show()
  }
})

defineExpose({ show, hide })
</script>

<style scoped>
.toast {
  position: fixed;
  top: 100px;
  left: 50%;
  transform: translateX(-50%);
  padding: 16px 24px;
  background: rgba(15, 12, 41, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  gap: 12px;
  z-index: 9999;
  min-width: 200px;
  max-width: 400px;
  border: 1px solid rgba(100, 255, 218, 0.3);
}

.toast i {
  font-size: 20px;
}

.toast span {
  font-size: 15px;
  font-weight: 500;
  color: #ccd6f6;
}

.toast.success {
  border-color: #64ffda;
  box-shadow: 0 4px 20px rgba(100, 255, 218, 0.3);
}

.toast.success i {
  color: #64ffda;
}

.toast.info {
  border-color: #00d4ff;
  box-shadow: 0 4px 20px rgba(0, 212, 255, 0.3);
}

.toast.info i {
  color: #00d4ff;
}

.toast.warning {
  border-color: #ffa502;
  box-shadow: 0 4px 20px rgba(255, 165, 2, 0.3);
}

.toast.warning i {
  color: #ffa502;
}

.toast.error {
  border-color: #ff4757;
  box-shadow: 0 4px 20px rgba(255, 71, 87, 0.3);
}

.toast.error i {
  color: #ff4757;
}

.toast-fade-enter-active,
.toast-fade-leave-active {
  transition: all 0.3s ease;
}

.toast-fade-enter-from {
  opacity: 0;
  transform: translateX(-50%) translateY(-20px);
}

.toast-fade-leave-to {
  opacity: 0;
  transform: translateX(-50%) translateY(-20px);
}
</style>
