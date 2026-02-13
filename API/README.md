# Personal Finance API

A modern, enterprise-grade **.NET 10** Web API for personal financial management, built with **Clean Architecture** principles and CQRS pattern.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=.net)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-12+-336791?style=flat-square&logo=postgresql)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Features](#features)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

## Overview

Personal Finance API is a robust backend solution for managing personal finances. It provides a comprehensive set of APIs to handle transactions, categories, budgets, and financial projections with enterprise-level architecture patterns.

The project demonstrates best practices in .NET development including:

- Clean Architecture (4 layers)
- CQRS Pattern (Command Query Responsibility Segregation)
- Domain-Driven Design (DDD)
- Dependency Injection
- Global Exception Handling
- Fluent Validation
- Entity Framework Core with PostgreSQL

## Architecture

This project follows **Clean Architecture** with **4 distinct layers**:

```
PersonalFinanceAPI/
├── src/
│   ├── Domain/                    # Enterprise Business Rules
│   │   ├── Entities/              # Domain aggregates (Transaction, Category, RecurrentTransaction)
│   │   ├── ValueObjects/          # Money (immutable, currency-aware)
│   │   ├── Enums/                 # TransactionType, RecurrentPeriod
│   │   ├── Events/                # Domain events (TransactionAddedEvent)
│   │   └── Shared/                # Shared domain utilities
│   │
│   ├── Application/               # Application Business Rules (CQRS Layer)
│   │   ├── Features/
│   │   │   ├── Transactions/      # Transaction commands and queries
│   │   │   ├── RecurrentTransactions/
│   │   │   └── Shared/
│   │   ├── Commands/              # Command handlers
│   │   ├── Queries/               # Query handlers (GetTransaction, GetTransactions, GetBalanceProjection)
│   │   ├── Exceptions/            # Application-level exceptions
│   │   ├── Validators/            # FluentValidation rules
│   │   ├── Repositories/          # Repository interfaces
│   │
│   ├── Infrastructure/            # External Agencies & Frameworks
│   │   ├── Persistence/           # EF Core DbContext, migrations
│   │   ├── Repositories/          # Repository implementations
│   │   ├── DependencyInjection/   # IoC container configuration
│   │   └── Migrations/            # Database migrations
│   │
│   └── API/                       # HTTP Interface (Controllers/Endpoints)
│       ├── Endpoints/             # Minimal API endpoints
│       ├── Middleware/            # Global exception handling middleware
│       ├── Program.cs             # Application entry point & configuration
│       ├── appsettings.json       # Configuration
│       └── appsettings.Development.json
│
└── tests/
    ├── UnitTests/                 # Unit tests
    ├── IntegrationTests/          # Integration tests
    └── FunctionalTests/           # End-to-end tests
```

### Dependency Flow

```
API (Endpoints)
  ↓
Application (Commands/Queries via MediatR)
  ↓
Domain (Entities, ValueObjects)
  ↓
Infrastructure (EF Core, Repositories)
  ↓
PostgreSQL Database
```

## Technology Stack

### Core Framework

- **[.NET 10](https://dotnet.microsoft.com/download)** - Latest .NET runtime
- **[ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core)** - Web framework

### Architecture & Design Patterns

- **[MediatR](https://github.com/jbogard/MediatR)** - CQRS pattern implementation (v14.0.0)
- **[FluentValidation](https://fluentvalidation.net/)** - Declarative validation (v12.1.1)
- **Clean Architecture** - Layered architecture pattern
- **Domain-Driven Design** - Business logic at core

### Data Access

- **[Entity Framework Core 9](https://docs.microsoft.com/en-us/ef/core)** - ORM for .NET
- **[PostgreSQL 12+](https://www.postgresql.org/)** - Primary database
- **Repository Pattern** - Data access abstraction

### API Documentation

- **[Scalar](https://github.com/scalar/scalar)** (v2.12.24) - Modern API documentation UI

### Development Tools

- **[C# 14](https://docs.microsoft.com/en-us/dotnet/csharp/)** - Latest C# language features
- **Nullable Reference Types** - Type safety
- **Implicit Usings** - Cleaner code

## Features

### Transaction Management

- Create transactions with validation
- Retrieve transactions with filtering
- Support for income (credit) and expense (debit) transactions
- Transaction categorization
- Idempotency support via SHA256 hashing (duplicate detection)

### Financial Calculations

- **Balance Projection** - Calculate projected balances for upcoming months
  - Formula: `ProjectedBalance = CurrentBalance + Sum(Future Income) - Sum(Future Expenses)`
  - Configurable time period and per-category projections
- **Money Value Object** - Currency-aware monetary calculations with arithmetic operations

### Category Management

- Hierarchical category structure (parent-child relationships)
- Tag support for flexible transaction organization

### Recurrent Transactions

- Recurring transaction management
- Multiple recurrence periods (Daily, Weekly, Monthly, Yearly)

### Financial Planning

- Budget tracking by period (Monthly, Yearly)
- Budget vs. Actual analysis

### API Features

- CORS enabled (configurable)
- Health check endpoint (`/health`)
- Auto-generated OpenAPI documentation via Scalar
- Minimal APIs for high performance
- Global exception handling middleware
- Input validation with FluentValidation

## Project Structure

```
src/
├── Domain/
│   ├── Entities/
│   │   ├── Entity.cs              # Base entity with generic Id
│   │   ├── Transaction.cs         # Transaction aggregate root
│   │   ├── RecurrentTransaction.cs # Recurring transactions
│   │   └── Category.cs            # Transaction categories
│   ├── Enums/
│   │   ├── TransactionType.cs     # Income/Expense
│   │   └── RecurrentPeriod.cs     # Daily, Weekly, Monthly, Yearly
│   ├── ValueObjects/
│   │   └── Money.cs               # Currency-aware monetary value
│   └── Events/
│       └── TransactionAddedEvent.cs
│
├── Application/
│   ├── Features/
│   │   ├── Transactions/
│   │   └── RecurrentTransactions/
│   ├── Queries/
│   ├── Exceptions/
│   └── Validators/
│
├── Infrastructure/
│   ├── Persistence/
│   │   ├── PersonalFinanceDbContext.cs
│   │   └── Configuration/         # Entity configurations
│   ├── Repositories/
│   │   ├── TransactionRepository.cs
│   │   └── BaseRepository.cs
│   └── DependencyInjection/
│       └── ServiceCollectionExtensions.cs
│
└── API/
    ├── Endpoints/
    │   └── TransactionEndpoints.cs
    ├── Middleware/
    │   └── GlobalExceptionHandlingMiddleware.cs
    ├── Program.cs
    └── appsettings.json
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)
- **IDE**: Visual Studio 2022, VS Code, or JetBrains Rider (optional but recommended)

### Installation

#### 1. Clone the Repository

```bash
git clone https://github.com/your-username/PersonalFinanceAPI.git
cd PersonalFinanceAPI
```

#### 2. Restore Dependencies

```bash
dotnet restore
```

#### 3. Configure Database Connection

Edit `src/API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PersonalFinanceDB;Username=postgres;Password=your_password;"
  }
}
```

Or set via environment variables:

**Windows PowerShell:**

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=PersonalFinanceDB;Username=postgres;Password=your_password;"
```

**macOS/Linux:**

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=PersonalFinanceDB;Username=postgres;Password=your_password;"
```

#### 4. Apply Database Migrations

```bash
cd src/API
dotnet ef database update --project ../Infrastructure
```

This command will:

- Create the `PersonalFinanceDB` database
- Create all required tables (Transactions, Categories, etc.)
- Seed initial data if configured

#### 5. Run the API

```bash
cd src/API
dotnet run
```

The API will be available at:

- **API Base URL**: `https://localhost:5001` or `http://localhost:5000`
- **API Documentation (Scalar)**: `https://localhost:5001/scalar/v1`
- **Health Check**: `https://localhost:5001/health`

## API Endpoints

### Transaction Endpoints

| Method | Endpoint                                            | Description                       |
| ------ | --------------------------------------------------- | --------------------------------- |
| `POST` | `/api/transactions`                                 | Create a new transaction          |
| `GET`  | `/api/transactions/{id}`                            | Get transaction by ID             |
| `GET`  | `/api/transactions`                                 | Get all transactions with filters |
| `GET`  | `/api/transactions/balance-projection/{categoryId}` | Get balance projection            |

### Request/Response Examples

#### Create Transaction

**Request:**

```http
POST /api/transactions HTTP/1.1
Content-Type: application/json

{
  "title": "Grocery Shopping",
  "amount": {
    "value": 45.50,
    "currency": "USD"
  },
  "date": "2025-02-13",
  "type": "Expense",
  "categoryId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response:**

```json
{
  "id": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
  "title": "Grocery Shopping",
  "amount": {
    "value": 45.5,
    "currency": "USD"
  },
  "date": "2025-02-13",
  "type": "Expense",
  "categoryId": "550e8400-e29b-41d4-a716-446655440000",
  "createdAt": "2025-02-13T10:30:00Z"
}
```

#### Get Balance Projection

**Request:**

```http
GET /api/transactions/balance-projection/550e8400-e29b-41d4-a716-446655440000?months=6
```

**Response:**

```json
{
  "categoryId": "550e8400-e29b-41d4-a716-446655440000",
  "currentBalance": 1000.0,
  "projections": [
    {
      "month": "2025-03",
      "projectedBalance": 1450.0,
      "income": 2000.0,
      "expenses": 550.0
    },
    {
      "month": "2025-04",
      "projectedBalance": 1950.0,
      "income": 2000.0,
      "expenses": 500.0
    }
  ]
}
```

## 💻 Development

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/UnitTests
dotnet test tests/IntegrationTests
dotnet test tests/FunctionalTests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Running in Development Mode

```bash
cd src/API
dotnet run --launch-profile https
```

The API will run in development mode with:

- Full debug information
- OpenAPI documentation accessible
- Scalar UI for API exploration
- Hot reload enabled

### Code Style & Standards

This project follows:

- **C# Coding Standards** - Microsoft's C# coding conventions
- **Clean Code** - Readable, maintainable, and testable
- **SOLID Principles** - Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **DDD Principles** - Domain-first design approach
- **Nullable Reference Types** - For type safety

### Database Migrations

Create a new migration:

```bash
cd src/API
dotnet ef migrations add MigrationName --project ../Infrastructure
```

Apply migrations:

```bash
dotnet ef database update --project ../Infrastructure
```

Revert last migration:

```bash
dotnet ef migrations remove --project ../Infrastructure
```

## Project Layers Explained

### Domain Layer (`src/Domain`)

The innermost layer containing pure business logic with no external dependencies.

**Key Concepts:**

- **Entities**: Objects with identity (Transaction, Category, etc.)
- **Value Objects**: Immutable objects without identity (Money)
- **Aggregates**: Groups of entities treated as a single unit
- **Domain Events**: Events that represent state changes

```csharp
// Example: Transaction Entity
public class Transaction : Entity<Guid>
{
    public string Title { get; protected set; }
    public Money Amount { get; protected set; }
    public TransactionType Type { get; protected set; }
    public DateOnly Date { get; protected set; }
    public Guid CategoryId { get; protected set; }

    public static Transaction Create(
        string title,
        Money amount,
        DateOnly date,
        TransactionType type,
        Guid categoryId)
    {
        // Business logic and validation
        return new Transaction { /* ... */ };
    }
}
```

### Application Layer (`src/Application`)

Implements use cases and orchestrates domain logic using CQRS pattern.

**Key Components:**

- **Commands**: Write operations (CreateTransactionCommand)
- **Queries**: Read operations (GetTransactionsQuery, GetBalanceProjectionQuery)
- **Handlers**: Implementation of commands and queries
- **DTOs**: Data Transfer Objects for API communication
- **Validation**: FluentValidation rules

```csharp
// Example: CreateTransactionCommand
public class CreateTransactionCommand : IRequest<TransactionDto>
{
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
}

// Handler
public class CreateTransactionCommandHandler
    : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(
        CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        // Implement business logic
    }
}
```

### Infrastructure Layer (`src/Infrastructure`)

Implements external concerns: databases, repositories, dependency injection.

**Key Components:**

- **DbContext**: Entity Framework Core configuration
- **Repositories**: Data access abstraction
- **Migrations**: Database schema versioning
- **Dependency Injection**: Service registration

```csharp
// Repository implementation
public class TransactionRepository : ITransactionRepository
{
    private readonly PersonalFinanceDbContext _context;

    public async Task<Transaction> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}
```

### API Layer (`src/API`)

HTTP interface exposing endpoints and handling requests.

**Key Components:**

- **Minimal API Endpoints**: Lightweight HTTP handlers
- **Middleware**: Cross-cutting concerns (exception handling)
- **Program.cs**: Application startup and configuration

```csharp
// Minimal API endpoint
group.MapPost("/", CreateTransaction)
    .WithName("Create Transaction")
    .Produces(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

private static async Task<IResult> CreateTransaction(
    CreateTransactionCommand command,
    IMediator mediator,
    CancellationToken cancellationToken)
{
    var result = await mediator.Send(command, cancellationToken);
    return Results.Created($"/api/transactions/{result.Id}", result);
}
```

## Key Design Patterns

### CQRS (Command Query Responsibility Segregation)

Separates read and write operations into different models:

- **Commands**: Modify state (CreateTransactionCommand)
- **Queries**: Retrieve state (GetTransactionsQuery)

### Repository Pattern

Abstracts data access logic, decoupling business logic from database implementation.

### Dependency Injection

All dependencies are resolved through ASP.NET Core's built-in IoC container.

### Value Objects

Immutable objects like `Money` ensure type safety and encapsulation.

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PersonalFinanceDB;Username=postgres;Password=postgres;"
  }
}
```

### Environment-Specific Configuration

- `appsettings.json` - Default settings
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production settings (if present)

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## Additional Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern Documentation](https://martinfowler.com/bliki/CQRS.html)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [MediatR Documentation](https://github.com/jbogard/MediatR/wiki)
- [FluentValidation Documentation](https://fluentvalidation.net/)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
