# Personal Finance API - Clean Architecture

A modern, enterprise-grade .NET 9 Web API demonstrating **Clean Architecture** with a structured approach to building maintainable and scalable financial management systems.

## Architecture Overview

This project is organized into **4 clean architecture layers**:

```
PersonalFinanceAPI/
├── src/
│   ├── Domain/                      # Business logic layer
│   │   ├── Entities/               # Domain aggregates (Transaction, Category, Budget, Tag)
│   │   ├── ValueObjects/           # Money, immutable value objects
│   │   └── Enums/                  # TransactionType, TransactionStatus, BudgetPeriod
│   │
│   ├── Application/                # Use case orchestration
│   │   ├── Commands/               # Command handlers (CreateTransaction, ImportStatement)
│   │   ├── Queries/                # Query handlers (GetBalanceProjection)
│   │   ├── Strategies/             # Import strategies (OFX, CSV, MT940)
│   │   └── Exceptions/             # Application-level exceptions
│   │
│   ├── Infrastructure/             # Technical implementation
│   │   ├── Persistence/            # EF Core DbContext configuration
│   │   ├── Repositories/           # Data access abstraction
│   │   └── DependencyInjection/    # Service registration
│   │
│   └── API/                        # HTTP interface
│       ├── Endpoints/              # Minimal API endpoints
│       ├── Middleware/             # Global exception handling
│       ├── Program.cs              # Application entry point
│       └── appsettings.*.json      # Configuration files
```

## Key Features & Core Components

### Domain Layer

- **Transaction Aggregate**: Core financial transaction with status management
- **Category Entity**: Hierarchical categories with parent-child relationships
- **Tag Entity**: M:N relationship with transactions for flexible tagging
- **Budget Entity**: Period-based budget tracking (Monthly/Yearly)
- **Money Value Object**: Currency-aware monetary values with arithmetic operations
- **Idempotency Support**: SHA256 hashing for duplicate transaction detection

### Application Layer

#### CQRS Pattern with MediatR

**Commands:**

- `CreateTransactionCommand`: Creates a new transaction with validation
- `ImportStatementCommand`: Bulk imports transactions from statement files

**Queries:**

- `GetBalanceProjectionQuery`: Calculates projected balance for upcoming months
  - Formula: Balance = Sum(Income) - Sum(Expenses)
  - Supports configurable month count and projections by category

#### Validation

- FluentValidation rules for all commands
- Declarative validation with custom error messages
- Automatic validation pipeline integration

- **CQRS Pattern**: Separated read (Queries) and write (Commands) operations
- **MediatR Integration**: Decoupled request handlers
- **FluentValidation**: Declarative validation rules
- **Import Strategies**: Pluggable OFX, CSV, and MT940 file parsers
- **Exception Handling**: Custom application exceptions with error codes

### Infrastructure Layer

- **Entity Framework Core 9**: Modern ORM with PostgreSQL support
- **Repository Pattern**: Abstracted data access layer
- **Dependency Injection**: Automatic service registration
- **Migrations Support**: EF Core migrations for database versioning

### API Layer

- **Minimal APIs**: Lightweight, high-performance HTTP endpoints
- **Global Middleware**: Centralized exception handling
- **OpenAPI Support**: Auto-generated API documentation with Scalar
- **CORS Configuration**: Cross-origin resource sharing support
- **Health Checks**: Built-in health check endpoint
  ```

  ```

2. **Create Database & Run Migrations**

   ```bash
   cd src/API
   dotnet ef database update --project ../Modules/Modules.Finances/Infrastructure
   ```

3. **Run the API**

   ```bash
   cd src/API
   dotnet run
   ```

4. **Access OpenAPI Documentation**
   Navigate to `https://localhost:7001/scalar/v1` for interactive API documentation.

## 📝 API Examples

### Create Transaction

```bash
POST /api/v1/transactions
Content-Type: application/json

{
  "title": "Salary Deposit",
  "amount": 5000,
  "currency": "USD",
  "date": "2026-01-28",
  "type": 1,
  "categoryId": "550e8400-e29b-41d4-a716-446655440000",
  "isRecurrent": true,
  "tagIds": ["550e8400-e29b-41d4-a716-446655440001"]
}
```

### Get Balance Projection

````bash
GET /api/v1/transactions/balance-projection/550e8400-e29b-41d4-a716-446655440000?monthCount=12&startDate=2026-01-01
```Getting Started

### Prerequisites
- .NET 9 SDK
- PostgreSQL 12+
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd APIPersonalFinance
````

2. **Configure Database Connection**

   Update `src/API/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=PersonalFinanceDB_Dev;Username=postgres;Password=your_password;"
     }
   }
   ```

3. **Restore Dependencies**

   ```bash
   dotnet restore
   ```

4. **Apply Database Migrations**

   ```bash
   cd src/API
   dotnet ef database update
   ```

5. **Run the Application**
   API Endpoints

### Transactions

```bash
# Create a new transaction
POST /api/transactions
Content-Type: application/json

{
  "title": "Grocery Shopping",
  "amount": 50.00,
  "currency": "USD",
  "date": "2024-01-15T00:00:00Z",
  "type": 2,
  "categoryId": "550e8400-e29b-41d4-a716-446655440000",
  "isRecurrent": false,
  "tagIds": []
}

# Get balance projection
GET /api/transactions/balance-projection/{categoryId}
```

### Statement Imports

```bash
# Import transactions from OFX file
POST /api/imports
Content-Type: application/json

{
  "fileContent": "OFXHEADER...",
  "fileFormat": "OFX",
  "defaultCategoryId": "550e8400-e29b-41d4-a716-446655440000",
  "currency": "USD",
  "skipDuplicates": true
}
```

### Health Check

```bash
GET /health
```

## Database Schema Highlights

### Transactions Table

- **Columns**: Id, Title, Amount, Currency, Date, Type, Status, CategoryId, CreatedAt, UpdatedAt
- **Indices**: Status, Type, Date, CategoryId, CreatedAt
- **Constraints**: FK to Categories (Restrict delete)

### Categories Table

- **Columns**: Id, Name, Description, ParentCategoryId, CreatedAt, UpdatedAt
- **Features**: Hierarchical support with self-referencing FK
- **Indices**: ParentCategoryId

### Tags Table

- **Columns**: Id, Name, CreatedAt, UpdatedAt
- **Unique**: Name (case-insensitive)

### Transaction-Tags (M:N)

- **Junction**: TransactionId + TagId (composite key)
- **Cascade**: Delete when transaction deleted

### Budgets Table

- **Columns**: Id, Name, Period, StartDate, EndDate, LimitAmount, LimitCurrency, IsActive, CategoryId, CreatedAt, UpdatedAt
- **Features**: Period-based limits with active flag
- **Indices**: CategoryId, IsActive, Period

## Technology Stack

| Component         | Technology            | Version |
| ----------------- | --------------------- | ------- |
| Framework         | .NET                  | 9.0     |
| Language          | C#                    | 12+     |
| Database          | PostgreSQL            | Latest  |
| ORM               | Entity Framework Core | 9.0     |
| CQRS              | MediatR               | 12.2    |
| Validation        | FluentValidation      | 11.9    |
| API Documentation | Scalar                | 1.2     |

## Design Patterns Used

1. **Clean Architecture**: Separation of concerns across 4 layers
2. **CQRS**: Command Query Responsibility Segregation with MediatR
3. **Repository Pattern**: Data access abstraction
4. **Strategy Pattern**: Pluggable import format handlers
5. **Value Objects**: Immutable, meaningful domain concepts
6. **Aggregate Root**: Transaction as the transaction aggregate
7. **Dependency Injection**: Automatic service resolution
8. **Middleware Pipeline**: Centralized cross-cutting concerns

## Development Workflow

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Formatting Code

```bash
dotnet format
```

### Applying Database Migrations

```bash
cd src/API
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Configuration Files

### `appsettings.json` (Production)

- Logging level: Information
- Database: Production PostgreSQL instance
- CORS: Restricted origins

### `appsettings.Development.json` (Development)

- Logging level: Debug
- Database: Local PostgreSQL instance
- CORS: All origins allowed

## Error Handling

The API uses standardized error responses:

```json
{
  "message": "Error description",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

HTTP Status Codes:

- `200 OK`: Successful request
- `201 Created`: Resource created
- `400 Bad Request`: Validation error
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Unhandled exception

## Security Considerations

- ✅ Input validation with FluentValidation
- ✅ Idempotency hash for duplicate detection
- ✅ Foreign key constraints for referential integrity
- ✅ Soft-delete patterns where needed
- ⚠️ TODO: Add authentication/authorization
- ⚠️ TODO: Add rate limiting
- ⚠️ TODO: Add audit logging

## Future Enhancements

- [ ] Authentication with JWT tokens
- [ ] Role-based authorization
- [ ] Audit logging for all transactions
- [ ] CSV and MT940 import strategy implementations
- [ ] Budget alerts and notifications
- [ ] Analytics module for financial insights
- [ ] Unit and integration tests
- [ ] API versioning strategy
- [ ] GraphQL endpoint alternative

---

**Last Updated**: January 2024  
**Architecture Version**: Clean Architecture v1.0  
**.NET Version**: 9.0
