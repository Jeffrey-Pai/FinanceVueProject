export interface AuthResponse {
  token: string
  username: string
  userId: number
}

export interface AccountDto {
  id: number
  accountNumber: string
  accountType: string
  balance: number
  createdAt: string
}

export interface TransactionDto {
  id: number
  type: string
  amount: number
  description?: string
  createdAt: string
  fromAccountId: number
  fromAccountNumber: string
  toAccountId?: number
  toAccountNumber?: string
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}
