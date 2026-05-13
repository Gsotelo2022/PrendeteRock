import { createRouter, createWebHistory } from 'vue-router'
import Login from './components/Login.vue'
import Register from './components/Register.vue'

const routes = [
  {
    path: '/login',
    component: Login,
    name: 'Login'
  },
  {
    path: '/register',
    component: Register,
    name: 'Register'
  },
  {
    path: '/home',
    component: () => import('./components/home/Home.vue'),
    name: 'Home'
  },
  {
    path: '/admin',
    component: () => import('./components/administrador/Dashboard.vue'),
    name: 'AdminDashboard'
  },
  {
    path: '/admin/productos',
    component: () => import('./components/administrador/GestionProductos.vue'),
    name: 'AdminProductos'
  },
  {
    path: '/admin/clientes',
    component: () => import('./components/administrador/GestionClientes.vue'),
    name: 'AdminClientes'
  },
  {
    path: '/admin/cupones',
    component: () => import('./components/administrador/GestionCupones.vue'),
    name: 'AdminCupones'
  },
  {
    path: '/cliente/mis-diseños',
    component: () => import('./components/cliente/MyDesigns.vue'),
    name: 'MyDesigns'
  },
  {
    path: '/cliente/crear-orden',
    component: () => import('./components/cliente/CreateOrder.vue'),
    name: 'CreateOrder'
  },
  {
    path: '/cliente/mis-ordenes',
    component: () => import('./components/cliente/MyOrders.vue'),
    name: 'MyOrders'
  },
  {
    path: '/',
    redirect: '/home'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
