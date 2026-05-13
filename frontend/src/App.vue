<template>
  <Sidebar>
    <router-view />
  </Sidebar>
</template>

<script setup>
import Sidebar from './components/Sidebar.vue'
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}
</style>
