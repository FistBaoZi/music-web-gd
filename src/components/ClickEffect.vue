<template>
  <div class="click-effects-container">
    <div 
      v-for="effect in effects" 
      :key="effect.id"
      class="click-effect"
      :style="{
        left: effect.x + 'px',
        top: effect.y + 'px'
      }"
    >
      <div class="ripple"></div>
      <div class="particles">
        <span v-for="i in 6" :key="i" class="particle" :style="getParticleStyle(i)"></span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const effects = ref([])
let effectId = 0

const getParticleStyle = (index) => {
  const angle = (index * 60) * Math.PI / 180
  const distance = 30 + Math.random() * 20
  return {
    '--tx': Math.cos(angle) * distance + 'px',
    '--ty': Math.sin(angle) * distance + 'px',
    '--delay': (Math.random() * 0.1) + 's'
  }
}

const handleClick = (e) => {
  const effect = {
    id: effectId++,
    x: e.clientX,
    y: e.clientY
  }
  
  effects.value.push(effect)
  
  // 动画结束后移除
  setTimeout(() => {
    effects.value = effects.value.filter(eff => eff.id !== effect.id)
  }, 800)
}

onMounted(() => {
  document.addEventListener('click', handleClick)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClick)
})
</script>

<style scoped>
.click-effects-container {
  pointer-events: none;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 9999;
}

.click-effect {
  position: absolute;
  transform: translate(-50%, -50%);
}

.ripple {
  width: 30px;
  height: 30px;
  border: 2px solid #64ffda;
  border-radius: 50%;
  animation: ripple 0.6s ease-out forwards;
}

@keyframes ripple {
  0% {
    transform: scale(0);
    opacity: 1;
  }
  100% {
    transform: scale(2.5);
    opacity: 0;
  }
}

.particles {
  position: absolute;
  top: 0;
  left: 0;
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: linear-gradient(135deg, #64ffda, #00d4ff);
  border-radius: 50%;
  box-shadow: 0 0 6px rgba(100, 255, 218, 0.8);
  animation: particle 0.8s ease-out forwards;
  animation-delay: var(--delay);
}

@keyframes particle {
  0% {
    transform: translate(0, 0) scale(1);
    opacity: 1;
  }
  100% {
    transform: translate(var(--tx), var(--ty)) scale(0);
    opacity: 0;
  }
}
</style>
