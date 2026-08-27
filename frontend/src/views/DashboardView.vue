<template>
  <div class="dashboard">
    <header class="navbar">
      <h1>💳 Bank Demo</h1>
      <div class="user-info">
        <span>Welcome, {{ authStore.username }}</span>
        <button @click="handleLogout" class="logout-btn">Logout</button>
      </div>
    </header>

    <main class="content">
      <section class="accounts-section">
        <h2>My Accounts</h2>
        <div v-if="accountStore.loading" class="loading">Loading accounts...</div>
        <div v-else-if="accountStore.error" class="error">{{ accountStore.error }}</div>
        <div v-else class="accounts-grid">
          <div
            v-for="account in accountStore.accounts"
            :key="account.id"
            class="account-card"
            :class="{ selected: selectedAccountId === account.id }"
            @click="selectAccount(account.id)"
          >
            <div class="account-number">{{ account.accountNumber }}</div>
            <div class="account-type">{{ account.accountType }}</div>
            <div class="account-balance">${{ account.balance.toFixed(2) }}</div>
          </div>
        </div>
      </section>

      <section v-if="selectedAccountId" class="operations-section">
        <h2>Operations</h2>
        <div class="op-tabs">
          <button
            v-for="op in ['Deposit', 'Withdraw', 'Transfer']"
            :key="op"
            :class="{ active: activeOp === op }"
            @click="activeOp = op"
          >{{ op }}</button>
        </div>

        <form @submit.prevent="handleOperation" class="op-form">
          <div class="form-group">
            <label>Amount ($)</label>
            <input v-model.number="opAmount" type="number" min="0.01" step="0.01" required />
          </div>
          <div v-if="activeOp === 'Transfer'" class="form-group">
            <label>Destination Account ID</label>
            <input v-model.number="toAccountId" type="number" required />
          </div>
          <div class="form-group">
            <label>Description (optional)</label>
            <input v-model="opDescription" type="text" />
          </div>
          <p v-if="opError" class="error">{{ opError }}</p>
          <p v-if="opSuccess" class="success">{{ opSuccess }}</p>
          <button type="submit" :disabled="opLoading">
            {{ opLoading ? 'Processing...' : activeOp }}
          </button>
        </form>

        <div class="transactions-section">
          <h3>Transaction History</h3>
          <div v-if="txStore.loading" class="loading">Loading...</div>
          <div v-else-if="txStore.error" class="error">{{ txStore.error }}</div>
          <table v-else-if="txStore.result && txStore.result.items.length" class="tx-table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Type</th>
                <th>Amount</th>
                <th>From</th>
                <th>To</th>
                <th>Description</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="tx in txStore.result.items" :key="tx.id">
                <td>{{ new Date(tx.createdAt).toLocaleDateString() }}</td>
                <td><span :class="'badge badge-' + tx.type.toLowerCase()">{{ tx.type }}</span></td>
                <td>${{ tx.amount.toFixed(2) }}</td>
                <td>{{ tx.fromAccountNumber }}</td>
                <td>{{ tx.toAccountNumber || '-' }}</td>
                <td>{{ tx.description || '-' }}</td>
              </tr>
            </tbody>
          </table>
          <p v-else>No transactions yet.</p>
          <div v-if="txStore.result && txStore.result.totalCount > txStore.result.pageSize" class="pagination">
            <button @click="prevPage" :disabled="txPage === 1">Prev</button>
            <span>Page {{ txPage }} of {{ Math.ceil((txStore.result?.totalCount || 0) / 10) }}</span>
            <button @click="nextPage" :disabled="txPage >= Math.ceil((txStore.result?.totalCount || 0) / 10)">Next</button>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useAccountStore } from '../stores/accounts'
import { useTransactionStore } from '../stores/transactions'

const router = useRouter()
const authStore = useAuthStore()
const accountStore = useAccountStore()
const txStore = useTransactionStore()

const selectedAccountId = ref<number | null>(null)
const activeOp = ref('Deposit')
const opAmount = ref<number>(0)
const opDescription = ref('')
const toAccountId = ref<number>(0)
const opLoading = ref(false)
const opError = ref('')
const opSuccess = ref('')
const txPage = ref(1)

onMounted(() => {
  accountStore.fetchAccounts()
})

function selectAccount(id: number) {
  selectedAccountId.value = id
  txPage.value = 1
  txStore.fetchTransactions(id, 1)
}

watch(activeOp, () => {
  opError.value = ''
  opSuccess.value = ''
  opAmount.value = 0
  opDescription.value = ''
})

async function handleOperation() {
  if (!selectedAccountId.value) return
  opLoading.value = true
  opError.value = ''
  opSuccess.value = ''
  try {
    if (activeOp.value === 'Deposit') {
      await accountStore.deposit(selectedAccountId.value, opAmount.value, opDescription.value || undefined)
      opSuccess.value = `Deposited $${opAmount.value.toFixed(2)} successfully.`
    } else if (activeOp.value === 'Withdraw') {
      await accountStore.withdraw(selectedAccountId.value, opAmount.value, opDescription.value || undefined)
      opSuccess.value = `Withdrew $${opAmount.value.toFixed(2)} successfully.`
    } else if (activeOp.value === 'Transfer') {
      await accountStore.transfer(selectedAccountId.value, toAccountId.value, opAmount.value, opDescription.value || undefined)
      opSuccess.value = `Transferred $${opAmount.value.toFixed(2)} successfully.`
    }
    opAmount.value = 0
    opDescription.value = ''
    txStore.fetchTransactions(selectedAccountId.value, txPage.value)
  } catch (e: any) {
    opError.value = e.response?.data?.message || 'Operation failed'
  } finally {
    opLoading.value = false
  }
}

function prevPage() {
  if (txPage.value > 1 && selectedAccountId.value) {
    txPage.value--
    txStore.fetchTransactions(selectedAccountId.value, txPage.value)
  }
}

function nextPage() {
  if (selectedAccountId.value && txStore.result) {
    const maxPage = Math.ceil(txStore.result.totalCount / txStore.result.pageSize)
    if (txPage.value < maxPage) {
      txPage.value++
      txStore.fetchTransactions(selectedAccountId.value, txPage.value)
    }
  }
}

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.dashboard { min-height: 100vh; background: #f0f2f5; }
.navbar {
  background: #4f46e5;
  color: white;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.navbar h1 { margin: 0; font-size: 1.4rem; }
.user-info { display: flex; align-items: center; gap: 1rem; }
.logout-btn {
  background: rgba(255,255,255,0.2);
  border: 1px solid rgba(255,255,255,0.4);
  color: white;
  padding: 0.4rem 0.8rem;
  border-radius: 4px;
  cursor: pointer;
}
.content { max-width: 1100px; margin: 2rem auto; padding: 0 1rem; }
.accounts-section, .operations-section { margin-bottom: 2rem; }
h2 { color: #333; margin-bottom: 1rem; }
h3 { color: #444; margin-bottom: 1rem; }
.accounts-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(220px, 1fr)); gap: 1rem; }
.account-card {
  background: white;
  padding: 1.2rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
  cursor: pointer;
  border: 2px solid transparent;
  transition: border-color 0.2s;
}
.account-card.selected { border-color: #4f46e5; }
.account-card:hover { border-color: #a5b4fc; }
.account-number { font-family: monospace; font-size: 1rem; color: #555; }
.account-type { font-size: 0.85rem; color: #888; margin: 0.3rem 0; }
.account-balance { font-size: 1.5rem; font-weight: bold; color: #111; }
.operations-section {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}
.op-tabs { display: flex; gap: 0.5rem; margin-bottom: 1rem; }
.op-tabs button {
  padding: 0.4rem 1rem;
  border: 1px solid #ddd;
  background: white;
  border-radius: 4px;
  cursor: pointer;
}
.op-tabs button.active { background: #4f46e5; color: white; border-color: #4f46e5; }
.op-form { max-width: 400px; }
.form-group { margin-bottom: 0.8rem; }
label { display: block; margin-bottom: 0.2rem; font-size: 0.9rem; color: #555; }
input {
  width: 100%;
  padding: 0.5rem 0.7rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 0.95rem;
  box-sizing: border-box;
}
.op-form button {
  padding: 0.6rem 1.5rem;
  background: #4f46e5;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.95rem;
}
.op-form button:disabled { opacity: 0.6; }
.error { color: #dc2626; font-size: 0.85rem; }
.success { color: #16a34a; font-size: 0.85rem; }
.loading { color: #888; }
.transactions-section { margin-top: 2rem; }
.tx-table { width: 100%; border-collapse: collapse; font-size: 0.9rem; }
.tx-table th { background: #f8f9fa; padding: 0.6rem 0.8rem; text-align: left; border-bottom: 2px solid #eee; }
.tx-table td { padding: 0.6rem 0.8rem; border-bottom: 1px solid #f0f0f0; }
.badge { padding: 0.2rem 0.5rem; border-radius: 3px; font-size: 0.8rem; font-weight: 500; }
.badge-deposit { background: #dcfce7; color: #166534; }
.badge-withdrawal { background: #fee2e2; color: #991b1b; }
.badge-transfer { background: #dbeafe; color: #1e40af; }
.pagination { display: flex; gap: 1rem; align-items: center; margin-top: 1rem; }
.pagination button { padding: 0.3rem 0.8rem; cursor: pointer; }
.pagination button:disabled { opacity: 0.5; cursor: not-allowed; }
</style>
