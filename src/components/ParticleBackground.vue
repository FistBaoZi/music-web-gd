<template>
  <div class="particle-background">
    <canvas ref="canvas"></canvas>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const canvas = ref(null)
let ctx = null
let particles = []
let animationId = null

class Particle {
  constructor(canvasWidth, canvasHeight) {
    this.x = Math.random() * canvasWidth
    this.y = Math.random() * canvasHeight
    this.size = Math.random() * 2 + 0.5
    this.speedX = Math.random() * 0.5 - 0.25
    this.speedY = Math.random() * 0.5 - 0.25
    this.opacity = Math.random() * 0.5 + 0.2
    this.color = this.getRandomColor()
  }

  getRandomColor() {
    const colors = [
      'rgba(100, 255, 218, ',  // 青色
      'rgba(0, 212, 255, ',     // 亮蓝
      'rgba(204, 214, 246, ',   // 浅灰蓝
    ]
    return colors[Math.floor(Math.random() * colors.length)]
  }

  update(canvasWidth, canvasHeight) {
    this.x += this.speedX
    this.y += this.speedY

    // 边界检测
    if (this.x > canvasWidth) this.x = 0
    if (this.x < 0) this.x = canvasWidth
    if (this.y > canvasHeight) this.y = 0
    if (this.y < 0) this.y = canvasHeight
  }

  draw(ctx) {
    ctx.fillStyle = this.color + this.opacity + ')'
    ctx.beginPath()
    ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2)
    ctx.fill()

    // 添加发光效果
    ctx.shadowBlur = 10
    ctx.shadowColor = this.color + '0.8)'
  }
}

const init = () => {
  const canvasEl = canvas.value
  if (!canvasEl) return

  ctx = canvasEl.getContext('2d')
  
  // 设置画布大小
  const resizeCanvas = () => {
    canvasEl.width = window.innerWidth
    canvasEl.height = window.innerHeight
  }
  resizeCanvas()
  window.addEventListener('resize', resizeCanvas)

  // 创建粒子
  const particleCount = Math.floor((canvasEl.width * canvasEl.height) / 15000)
  for (let i = 0; i < particleCount; i++) {
    particles.push(new Particle(canvasEl.width, canvasEl.height))
  }

  // 动画循环
  const animate = () => {
    ctx.clearRect(0, 0, canvasEl.width, canvasEl.height)

    // 更新和绘制粒子
    particles.forEach(particle => {
      particle.update(canvasEl.width, canvasEl.height)
      particle.draw(ctx)
    })

    // 绘制连接线
    connectParticles()

    animationId = requestAnimationFrame(animate)
  }

  animate()
}

const connectParticles = () => {
  const maxDistance = 120

  for (let i = 0; i < particles.length; i++) {
    for (let j = i + 1; j < particles.length; j++) {
      const dx = particles[i].x - particles[j].x
      const dy = particles[i].y - particles[j].y
      const distance = Math.sqrt(dx * dx + dy * dy)

      if (distance < maxDistance) {
        const opacity = (1 - distance / maxDistance) * 0.15
        ctx.strokeStyle = `rgba(100, 255, 218, ${opacity})`
        ctx.lineWidth = 0.5
        ctx.beginPath()
        ctx.moveTo(particles[i].x, particles[i].y)
        ctx.lineTo(particles[j].x, particles[j].y)
        ctx.stroke()
      }
    }
  }
}

onMounted(() => {
  init()
})

onUnmounted(() => {
  if (animationId) {
    cancelAnimationFrame(animationId)
  }
  window.removeEventListener('resize', init)
})
</script>

<style scoped>
.particle-background {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 0;
  pointer-events: none;
  overflow: hidden;
}

canvas {
  display: block;
  width: 100%;
  height: 100%;
}
</style>
