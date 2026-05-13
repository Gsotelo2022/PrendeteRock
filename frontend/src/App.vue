<template>
  <Sidebar>
    <router-view />
  </Sidebar>
  <ToastContainer />
</template>

<script setup>
import Sidebar from './components/Sidebar.vue'
import ToastContainer from './components/ToastContainer.vue'
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}
</style>
