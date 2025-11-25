# Grain Broker Application

A full-stack grain brokerage management system built with .NET 8, demonstrating professional software architecture with clean separation of concerns, service layer pattern, and comprehensive testing.

---

## Overview

This application manages grain trading operations, tracking customers, suppliers, and orders with a RESTful API backend. It features:

- **Automated database setup** from CSV data
- **Service layer architecture** for business logic separation
- **RESTful API** with full CRUD operations
- **Comprehensive test coverage** (73 tests)
- **Interactive API documentation** with Swagger/OpenAPI

---

## Quick Start

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or Developer Edition)
- Git

### Setup in 3 Steps

1. **Clone the repository**
   ```bash
   git clone <your-repository-url>
   cd GrainBrokerApp
   ```

2. **Set up the database**
   ```powershell
   .\Setup-GrainBrokerDb.ps1
   ```

3. **Run the application**
   ```bash
   cd GrainBrokerAPI
   dotnet run
   ```

4. **Open Swagger UI**
   ```
   https://localhost:5001/swagger
   ```
---

## What Gets Created

- **Database**: `GrainBrokerDb` with normalized schema
- **Sample Data**:
  - 3 Customers
  - 4 Suppliers
  - 169 Orders
- **API Endpoints**: 15 RESTful endpoints across 3 controllers
- **Tests**: 73 comprehensive unit tests

---

## Architecture

```
┌─────────────────────────────────────┐
│     CSV Data (169 orders)          │
└─────────────┬───────────────────────┘
              │
              ▼
┌─────────────────────────────────────┐
│   PowerShell Setup Script           │
└─────────────┬───────────────────────┘
              │
              ▼
┌─────────────────────────────────────┐
│   SQL Server Database               │
│   • Customers                       │
│   • Orders                          │
│   • Suppliers                       │
└─────────────┬───────────────────────┘
              │
              ▼
┌─────────────────────────────────────┐
│   .NET 8 Web API                    │
│   ┌─────────────────────────────┐   │
│   │ Controllers (HTTP Layer)    │   │
│   └─────────────┬───────────────┘   │
│                 │                   │
│   ┌─────────────▼───────────────┐   │
│   │ Services (Business Logic)   │   │
│   └─────────────┬───────────────┘   │
│                 │                   │
│   ┌─────────────▼───────────────┐   │
│   │ DbContext (Data Access)     │   │
│   └─────────────────────────────┘   │
└─────────────────────────────────────┘
              │
              ▼
┌─────────────────────────────────────┐
│   Swagger UI / REST API             │
└─────────────────────────────────────┘
```
---

## API Endpoints

All endpoints are documented in Swagger UI at `https://localhost:5001/swagger`

### Customers
- `GET /api/customers` - List all customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create new customer
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

### Orders
- `GET /api/orders` - List all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}` - Update order
- `DELETE /api/orders/{id}` - Delete order

### Suppliers
- `GET /api/suppliers` - List all suppliers
- `GET /api/suppliers/{id}` - Get supplier by ID
- `POST /api/suppliers` - Create new supplier
- `PUT /api/suppliers/{id}` - Update supplier
- `DELETE /api/suppliers/{id}` - Delete supplier

---

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Expected output
Passed!  - Failed: 0, Passed: 73, Skipped: 0, Total: 73
```

## Future Enhancements

Potential next steps for this project:

- [ ] Angular frontend
- [ ] JWT authentication and authorization
- [ ] FluentValidation for advanced validation rules
- [ ] Structured logging
- [ ] Pagination and filtering for large datasets

---