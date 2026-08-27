import { defineStore } from 'pinia'
import { ref } from 'vue'
import { authApi } from '../api'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const username = ref<string | null>(localStorage.getItem('username'))
  const userId = ref<number | null>(Number(localStorage.getItem('userId')) || null)

  function setAuth(t: string, u: string, id: number) {
    token.value = t
    username.value = u
    userId.value = id
    localStorage.setItem('token', t)
    localStorage.setItem('username', u)
    localStorage.setItem('userId', String(id))
  }

  function clearAuth() {
    token.value = null
    username.value = null
    userId.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('username')
    localStorage.removeItem('userId')
  }

  async function register(u: string, email: string, pw: string) {
    const res = await authApi.register(u, email, pw)
    setAuth(res.data.token, res.data.username, res.data.userId)
  }

  async function login(u: string, pw: string) {
    const res = await authApi.login(u, pw)
    setAuth(res.data.token, res.data.username, res.data.userId)
  }

  function logout() {
    clearAuth()
  }

  const isLoggedIn = () => !!token.value

  return { token, username, userId, register, login, logout, isLoggedIn }
})
