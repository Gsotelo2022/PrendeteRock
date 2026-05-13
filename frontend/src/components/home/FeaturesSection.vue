<template>
  <section class="why-choose">
    <h2>¿Por Qué Elegir PrendeteRock?</h2>
    <div class="features-grid">
      <div class="feature-card">
        <div class="feature-icon ai"></div>
        <h3>Diseño Impulsado por IA</h3>
        <p>Genera diseños impresionantes con nuestra tecnología de IA avanzada</p>
      </div>
      <div class="feature-card">
        <div class="feature-icon upload"></div>
        <h3>Carga Fácil</h3>
        <p>Sube tus propias imágenes en segundos con arrastrar y soltar</p>
      </div>
      <div class="feature-card">
        <div class="feature-icon formats"></div>
        <h3>Múltiples Formatos</h3>
        <p>Elige entre pósters, lienzo, metal, acrílico y más</p>
      </div>
      <div class="feature-card">
        <div class="feature-icon production"></div>
        <h3>Producción Rápida</h3>
        <p>Tiempos de entrega rápidos sin comprometer la calidad</p>
      </div>
      <div class="feature-card">
        <div class="feature-icon quality"></div>
        <h3>Garantía de Calidad</h3>
        <p>Materiales premium y estándares de impresión profesionales</p>
      </div>
      <div class="feature-card">
        <div class="feature-icon tracking"></div>
        <h3>Seguir Pedidos</h3>
        <p>Monitorea el estado de tu pedido en tiempo real</p>
      </div>
    </div>
  </section>
</template>

<script>
export default {
  name: 'FeaturesSection',
}
</script>

<style scoped>
.why-choose {
  padding: 80px 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.why-choose h2 {
  font-size: 2.5rem;
  font-weight: 800;
  text-align: center;
  margin-bottom: 60px;
  color: #1a1a1a;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 30px;
}

.feature-card {
  background: white;
  padding: 40px 30px;
  border-radius: 20px;
  text-align: center;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.05);
}

.feature-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.1);
}

.feature-icon {
  width: 70px;
  height: 70px;
  margin: 0 auto 20px;
  border-radius: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2.5rem;
}

.feature-icon.ai {
  background: linear-gradient(135deg, #a020f0 0%, #c946ff 100%);
}

.feature-icon.upload {
  background: linear-gradient(135deg, #ff006e 0%, #ff3399 100%);
}

.feature-icon.formats {
  background: linear-gradient(135deg, #ff7a00 0%, #ffaa33 100%);
}

.feature-icon.production {
  background: linear-gradient(135deg, #0066ff 0%, #3399ff 100%);
}

.feature-icon.quality {
  background: linear-gradient(135deg, #00cc44 0%, #33ff77 100%);
}

.feature-icon.tracking {
  background: linear-gradient(135deg, #6633ff 0%, #9966ff 100%);
}

.feature-card h3 {
  font-size: 1.3rem;
  font-weight: 700;
  margin-bottom: 10px;
  color: #1a1a1a;
}

.feature-card p {
  color: #666;
  font-size: 0.95rem;
  line-height: 1.5;
}

@media (max-width: 768px) {
  .why-choose h2 {
    font-size: 2rem;
  }

  .features-grid {
    grid-template-columns: 1fr;
  }
}
</style>
