# Bank Demo Project

A full-stack bank demo application with:
- **Frontend**: Vue 3 + TypeScript + Pinia + Axios
- **Backend**: .NET 8 Web API + EF Core + MSSQL + JWT + FluentValidation + Swagger
- **Infrastructure**: Docker Compose

## Features

- User registration and login with JWT authentication
- View bank accounts and balances
- Deposit, withdraw, and transfer money (wrapped in database transactions)
- Paginated transaction history

## Architecture

### Backend (layered)
```
Controller -> Service -> Repository -> EF Core -> MSSQL
```

### Project Structure
```
.
├── backend/          # .NET 8 Web API
│   ├── Controllers/  # Auth, Accounts, Transactions
│   ├── Services/     # Business logic
│   ├── Repositories/ # Data access layer
│   ├── Models/       # Domain entities
│   ├── DTOs/         # Data transfer objects
│   ├── Validators/   # FluentValidation validators
│   ├── Data/         # EF Core DbContext + Migrations
│   └── Program.cs    # DI, Auth, Swagger setup
├── frontend/         # Vue 3 + TypeScript SPA
│   └── src/
│       ├── api/      # Axios client
│       ├── stores/   # Pinia (auth, accounts, transactions)
│       ├── views/    # Login, Register, Dashboard
│       └── types/    # TypeScript interfaces
├── docker-compose.yml
└── .env.example
```

## Quick Start with Docker

1. **Set up your environment file:**
   ```bash
   cp .env.example .env
   # Edit .env and set a strong MSSQL_SA_PASSWORD and JWT_KEY
   ```

2. **Configure the connection strings:**  
   In `docker-compose.yml` and `backend/appsettings*.json`, replace `******` with your
   MSSQL SA password (the same value set for `MSSQL_SA_PASSWORD` in your `.env`).  
   The connection string format is:
   ```
   Server=sqlserver,1433;Database=BankDb;User Id=sa;******;TrustServerCertificate=True;
   ```

3. **Run with Docker Compose:**
   ```bash
   docker-compose up --build
   ```

4. **Access the application:**
   - Frontend: http://localhost:3000
   - API + Swagger: http://localhost:8080/swagger

## Local Development

### Backend
```bash
cd backend
# Ensure MSSQL is running (e.g. docker-compose up sqlserver -d)
dotnet run
```

### Frontend
```bash
cd frontend
npm install
npm run dev   # http://localhost:5173
```

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | /api/auth/register | No | Register a new user |
| POST | /api/auth/login | No | Login and receive JWT |
| GET | /api/accounts | JWT | List accounts for user |
| POST | /api/accounts/deposit | JWT | Deposit to account |
| POST | /api/accounts/withdraw | JWT | Withdraw from account |
| POST | /api/accounts/transfer | JWT | Transfer between accounts |
| GET | /api/transactions/{accountId}?page=1&pageSize=10 | JWT | Paginated history |

## Environment Variables

| Variable | Description |
|----------|-------------|
| `MSSQL_SA_PASSWORD` | SQL Server SA password (min 8 chars, mixed case + symbol) |
| `JWT_KEY` | JWT signing key (use a long random string in production) |

## Security Notes

> This is a **demo project**. Before using in production:
> - Replace the JWT key with a cryptographically random value
> - Restrict CORS origins to your actual domain
> - Restrict Swagger to development environments only
> - Use secrets management (Azure Key Vault, AWS Secrets Manager, etc.) for all credentials
