# Grain Broker Application
## Visual Architecture

```
╔═══════════════════════════════════════════════════════════════════════════╗
║                           DATA LAYER                                      ║
╚═══════════════════════════════════════════════════════════════════════════╝

     GrainBrokerDataset.csv
     ├─ 169 grain orders
     ├─ 3 customers
     └─ 4 suppliers
              │
              │ Parsed and Validated
              ▼
     Setup-GrainBrokerDb.ps1
     ├─ Drops/Creates database
     ├─ Creates normalized schema
     ├─ Imports CSV data
     └─ Validates relationships
              │
              │ SQLCMD Execution
              ▼
     SQL Server (GrainBrokerDb)
     ┌──────────────────────────────┐
     │  Customers ─┐                │
     │  (3 records)│                │
     │             ├──► GrainOrders │
     │  Suppliers ─┘    (169 records│
     │  (4 records)                 │
     └──────────────────────────────┘

════════════════════════════════════════════════════════════════════════════

╔═══════════════════════════════════════════════════════════════════════════╗
║                        BACKEND API (.NET 8)                               ║
╚═══════════════════════════════════════════════════════════════════════════╝

     Program.cs (Startup Configuration)
     ├─ Registers DbContext
     ├─ Registers Services (DI)
     ├─ Configures Swagger
     └─ Enables CORS
              │
              ▼
     ┌───────────────────────────────────────────────────────────────┐
     │                    LAYERED ARCHITECTURE                       │
     └───────────────────────────────────────────────────────────────┘

     ┌─────────────────────────────────────────────────────────────┐
     │  LAYER 1: Data Access (Entity Framework Core)               │
     ├─────────────────────────────────────────────────────────────┤
     │  GrainBrokerContext (DbContext)                             │
     │  ├─ DbSet<Customer>                                         │
     │  ├─ DbSet<Order>                                            │
     │  └─ DbSet<Supplier>                                         │
     │                                                             │
     │  Models (POCOs)                                             │
     │  ├─ Customer.cs    (Id, Location)                           │
     │  ├─ Order.cs       (Id, OrderDate, CustomerId, etc.)        │
     │  └─ Supplier.cs    (Id, Location)                           │
     └─────────────────────────────────────────────────────────────┘
                              │
                              │ Injected via DI
                              ▼
     ┌─────────────────────────────────────────────────────────────┐
     │  LAYER 2: Business Logic (Service Layer)                    │     
     ├─────────────────────────────────────────────────────────────┤
     │  Service Interfaces                                         │        
     │  ├─ ICustomerService                                        │
     │  ├─ IOrderService                                           │
     │  └─ ISupplierService                                        │
     │                                                             │
     │  Service Implementations                                    │
     │  ├─ CustomerService                                         │
     │  │   • Validates location required                          │
     │  │   • Handles customer business logic                      │
     │  │                                                          │
     │  ├─ OrderService                                            │
     │  │   • Validates order amounts > 0                          │
     │  │   • Validates cost >= 0                                  │
     │  │   • Enforces business rules                              │
     │  │                                                          │
     │  └─ SupplierService                                         │
     │      • Validates location required                          │
     │      • Handles supplier business logic                      │
     └─────────────────────────────────────────────────────────────┘
                              │
                              │ Injected via DI
                              ▼
     ┌─────────────────────────────────────────────────────────────┐
     │  LAYER 3: API Controllers (HTTP Endpoints)                  │
     ├─────────────────────────────────────────────────────────────┤
     │  CustomersController                                        │
     │  ├─ GET    /api/customers      (List all)                   │
     │  ├─ GET    /api/customers/{id} (Get one)                    │
     │  ├─ POST   /api/customers      (Create)                     │
     │  ├─ PUT    /api/customers/{id} (Update)                     │
     │  └─ DELETE /api/customers/{id} (Delete)                     │
     │                                                             │
     │  OrdersController                                           │
     │  ├─ GET    /api/orders         (List all)                   │
     │  ├─ GET    /api/orders/{id}    (Get one)                    │
     │  ├─ POST   /api/orders         (Create)                     │
     │  ├─ PUT    /api/orders/{id}    (Update)                     │
     │  └─ DELETE /api/orders/{id}    (Delete)                     │
     │                                                             │
     │  SuppliersController                                        │
     │  ├─ GET    /api/suppliers      (List all)                   │
     │  ├─ GET    /api/suppliers/{id} (Get one)                    │
     │  ├─ POST   /api/suppliers      (Create)                     │
     │  ├─ PUT    /api/suppliers/{id} (Update)                     │
     │  └─ DELETE /api/suppliers/{id} (Delete)                     │
     └─────────────────────────────────────────────────────────────┘
                              │
                              │ JSON over HTTP/HTTPS
                              ▼
     ┌─────────────────────────────────────────────────────────────┐
     │  Swagger UI (API Documentation)                             │
     │  https://localhost:5001/swagger                             │
     │  • Interactive API testing                                  │
     │  • Request/response examples                                │
     │  • Schema documentation                                     │
     └─────────────────────────────────────────────────────────────┘

════════════════════════════════════════════════════════════════════════════

╔═══════════════════════════════════════════════════════════════════════════╗
║                        FRONTEND (NEXT PHASE)                              ║
╚═══════════════════════════════════════════════════════════════════════════╝

     Angular Application
     ├─ Customer Management UI
     ├─ Order Management UI
     ├─ Supplier Management UI
     └─ Dashboard & Reports

```

