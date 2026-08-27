import { defineStore } from 'pinia'
import { ref } from 'vue'
import { accountsApi } from '../api'
import type { AccountDto } from '../types'

export const useAccountStore = defineStore('accounts', () => {
  const accounts = ref<AccountDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchAccounts() {
    loading.value = true
    error.value = null
    try {
      const res = await accountsApi.getAccounts()
      accounts.value = res.data
    } catch (e: any) {
      error.value = e.response?.data?.message || 'Failed to load accounts'
    } finally {
      loading.value = false
    }
  }

  async function deposit(accountId: number, amount: number, description?: string) {
    const res = await accountsApi.deposit(accountId, amount, description)
    const idx = accounts.value.findIndex((a) => a.id === accountId)
    if (idx !== -1) accounts.value[idx] = res.data
  }

  async function withdraw(accountId: number, amount: number, description?: string) {
    const res = await accountsApi.withdraw(accountId, amount, description)
    const idx = accounts.value.findIndex((a) => a.id === accountId)
    if (idx !== -1) accounts.value[idx] = res.data
  }

  async function transfer(fromAccountId: number, toAccountId: number, amount: number, description?: string) {
    await accountsApi.transfer(fromAccountId, toAccountId, amount, description)
    await fetchAccounts()
  }

  return { accounts, loading, error, fetchAccounts, deposit, withdraw, transfer }
})
