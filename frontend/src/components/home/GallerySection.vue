<template>
  <section class="featured-prints">
    <h2>Impresiones Destacadas</h2>
    <div class="gallery-grid">
      <div class="gallery-item">
        <img src="https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=400&h=300&fit=crop" alt="Mountain sunset">
      </div>
      <div class="gallery-item">
        <img src="https://images.unsplash.com/photo-1441974231531-c6227db76b6e?w=400&h=300&fit=crop" alt="Forest path">
      </div>
      <div class="gallery-item">
        <img src="https://images.unsplash.com/photo-1470114716159-e389f8712fda?w=400&h=300&fit=crop" alt="Beautiful landscape">
      </div>
    </div>
  </section>
</template>

<script>
export default {
  name: 'GallerySection',
}
</script>

<style scoped>
.featured-prints {
  padding: 80px 20px;
  background-color: #fff;
  max-width: 1200px;
  margin: 0 auto;
}

.featured-prints h2 {
  font-size: 2.5rem;
  font-weight: 800;
  text-align: center;
  margin-bottom: 60px;
  color: #1a1a1a;
}

.gallery-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 20px;
}

.gallery-item {
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  height: 250px;
}

.gallery-item:hover {
  transform: scale(1.05);
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.15);
}

.gallery-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

@media (max-width: 768px) {
  .featured-prints h2 {
    font-size: 2rem;
  }

  .gallery-grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 480px) {
  .featured-prints {
    padding: 40px 20px;
  }

  .gallery-grid {
    grid-template-columns: 1fr;
  }
}
</style>
