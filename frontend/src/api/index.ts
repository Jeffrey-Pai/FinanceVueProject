import api from './axios'
import type { AuthResponse, AccountDto, TransactionDto, PagedResult } from '../types'

export const authApi = {
  register: (username: string, email: string, password: string) =>
    api.post<AuthResponse>('/auth/register', { username, email, password }),
  login: (username: string, password: string) =>
    api.post<AuthResponse>('/auth/login', { username, password })
}

export const accountsApi = {
  getAccounts: () => api.get<AccountDto[]>('/accounts'),
  deposit: (accountId: number, amount: number, description?: string) =>
    api.post<AccountDto>('/accounts/deposit', { accountId, amount, description }),
  withdraw: (accountId: number, amount: number, description?: string) =>
    api.post<AccountDto>('/accounts/withdraw', { accountId, amount, description }),
  transfer: (fromAccountId: number, toAccountId: number, amount: number, description?: string) =>
    api.post('/accounts/transfer', { fromAccountId, toAccountId, amount, description })
}

export const transactionsApi = {
  getTransactions: (accountId: number, page = 1, pageSize = 10) =>
    api.get<PagedResult<TransactionDto>>(`/transactions/${accountId}`, { params: { page, pageSize } })
}
