import { defineStore } from 'pinia'
import { ref } from 'vue'
import { transactionsApi } from '../api'
import type { TransactionDto, PagedResult } from '../types'

export const useTransactionStore = defineStore('transactions', () => {
  const result = ref<PagedResult<TransactionDto> | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchTransactions(accountId: number, page = 1, pageSize = 10) {
    loading.value = true
    error.value = null
    try {
      const res = await transactionsApi.getTransactions(accountId, page, pageSize)
      result.value = res.data
    } catch (e: any) {
      error.value = e.response?.data?.message || 'Failed to load transactions'
    } finally {
      loading.value = false
    }
  }

  return { result, loading, error, fetchTransactions }
})
