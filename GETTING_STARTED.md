# Getting Started with Grain Broker Application

A step-by-step guide to set up and run the Grain Broker API locally.

---

## Prerequisites

Before you begin, ensure you have the following installed:

**Git** - [Download](https://git-scm.com/downloads)
**.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
**SQL Server** (LocalDB, Express, or Developer Edition) - [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
**SQL Server Management Studio (SSMS)** - Optional - [Download](https://aka.ms/ssmsfullsetup)


## Quick Start

### Step 1: Clone the Repository

```bash
# Clone from Git
git clone <your-repository-url>

# Navigate to project directory
cd GrainBrokerApp
```
---

### Step 2: Set Up the Database

The project includes an automated PowerShell script that:
- Creates the SQL Server database
- Sets up the schema
- Imports sample data from CSV

#### Option A: Using Default Settings (localhost)

```powershell
# Run the setup script
.\Setup-GrainBrokerDb.ps1
```

#### Option B: Using Custom SQL Server Instance

1. Open `Setup-GrainBrokerDb.ps1` in a text editor
2. Modify the `$ServerName` variable (line 2):
   ```powershell
   $ServerName = "localhost"  # Change to your SQL Server instance name
   ```
3. Save and run:
   ```powershell
   .\Setup-GrainBrokerDb.ps1
   ```

#### What You Should See:

```
=== Grain Broker Database Setup ===

Dropping and recreating database 'GrainBrokerDb'...
Creating database schema...
Importing data from CSV...
Found 3 unique customers and 4 unique suppliers
Inserting customers...
Inserting suppliers...
Inserting orders (this may take a moment)...
  Inserted 20 orders...
  Inserted 40 orders...
  ...

=== Setup Complete ===
Database: GrainBrokerDb
Customers: 3
Suppliers: 4
Orders: 169

You can now run your GrainBrokerAPI application!
```

---

### Step 3: Update Connection String (if needed)

If you're not using `localhost`, update the connection string:

1. Open `GrainBrokerAPI/appsettings.json`
2. Update the connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=GrainBrokerDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

---

### Step 4: Run the Application

```bash
# Navigate to the API project
cd GrainBrokerAPI

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

#### What You Should See:

```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

The API is now running! 🎉

---

## 🧪 Testing the API

### Method 1: Using Swagger UI (Recommended)

Swagger provides an interactive web interface to test your API.

1. **Open your browser** and navigate to:
   ```
   https://localhost:5001/swagger
   ```

2. **You'll see all available endpoints:**
   - **Customers**
     - GET `/api/customers` - List all customers
     - GET `/api/customers/{id}` - Get customer by ID
     - POST `/api/customers` - Create new customer
     - PUT `/api/customers/{id}` - Update customer
     - DELETE `/api/customers/{id}` - Delete customer

   - **Orders**
     - GET `/api/orders` - List all orders
     - GET `/api/orders/{id}` - Get order by ID
     - POST `/api/orders` - Create new order
     - PUT `/api/orders/{id}` - Update order
     - DELETE `/api/orders/{id}` - Delete order

   - **Suppliers**
     - GET `/api/suppliers` - List all suppliers
     - GET `/api/suppliers/{id}` - Get supplier by ID
     - POST `/api/suppliers` - Create new supplier
     - PUT `/api/suppliers/{id}` - Update supplier
     - DELETE `/api/suppliers/{id}` - Delete supplier

3. **Try an endpoint:**
   - Click on **GET `/api/customers`**
   - Click **"Try it out"**
   - Click **"Execute"**
   - See the response below!


**Happy exploring!**
