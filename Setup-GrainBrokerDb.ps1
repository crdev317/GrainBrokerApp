# Config
$ServerName = "localhost"  # Change this to match your SQL Server instance
$DatabaseName = "GrainBrokerDb"
$CsvPath = "C:\git\Enate\GrainBrokerApp\GrainBrokerDataset.csv"

Write-Host "`n=== Grain Broker Database Setup ===" -ForegroundColor Cyan

# Drop and create the database
$DropAndCreateDb = @"
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'$DatabaseName')
BEGIN
    ALTER DATABASE [$DatabaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$DatabaseName];
END

CREATE DATABASE [$DatabaseName];
GO
"@

Write-Host "`nDropping and recreating database '$DatabaseName'..." -ForegroundColor Yellow
sqlcmd -S $ServerName -E -Q $DropAndCreateDb

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to create database. Please check your SQL Server connection." -ForegroundColor Red
    exit 1
}

# Create schema
$CreateSchema = @"
USE [$DatabaseName];
GO

-- Customers table
CREATE TABLE [dbo].[Customers](
    [Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
    [Location] [nvarchar](200) NOT NULL
);

-- Suppliers table
CREATE TABLE [dbo].[Suppliers](
    [Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
    [Location] [nvarchar](200) NOT NULL
);

-- GrainOrders table
CREATE TABLE [dbo].[GrainOrders](
    [Id] [uniqueidentifier] NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [OrderDate] [datetime] NOT NULL,
    [PurchaseOrder] [uniqueidentifier] NOT NULL,
    [CustomerId] [uniqueidentifier] NOT NULL,
    [SupplierId] [uniqueidentifier] NOT NULL,
    [OrderReqAmtTon] [int] NOT NULL,
    [SuppliedAmtTon] [int] NOT NULL,
    [CostOfDelivery] [decimal](18,2) NOT NULL,
    CONSTRAINT [FK_GrainOrders_Customer] FOREIGN KEY ([CustomerId])
        REFERENCES [dbo].[Customers]([Id]),
    CONSTRAINT [FK_GrainOrders_Supplier] FOREIGN KEY ([SupplierId])
        REFERENCES [dbo].[Suppliers]([Id])
);
GO
"@

Write-Host "Creating database schema..." -ForegroundColor Yellow
sqlcmd -S $ServerName -E -Q $CreateSchema

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to create schema." -ForegroundColor Red
    exit 1
}

# Import data from CSV
Write-Host "Importing data from CSV..." -ForegroundColor Yellow

if (-not (Test-Path $CsvPath)) {
    Write-Host "CSV file not found at: $CsvPath" -ForegroundColor Red
    exit 1
}

# Read CSV and extract unique customers and suppliers
$csvData = Import-Csv -Path $CsvPath

# Extract unique customers
$customers = $csvData | Select-Object @{Name='Id';Expression={$_.'Customer ID'}}, @{Name='Location';Expression={$_.'Customer Location'}} | Sort-Object Id -Unique

# Extract unique suppliers
$suppliers = $csvData | Select-Object @{Name='Id';Expression={$_.'Fullfilled By ID'}}, @{Name='Location';Expression={$_.'Fullfilled By Location'}} | Sort-Object Id -Unique

Write-Host "Found $($customers.Count) unique customers and $($suppliers.Count) unique suppliers" -ForegroundColor Green

# Insert Customers
Write-Host "Inserting customers..." -ForegroundColor Yellow
foreach ($customer in $customers) {
    $insertCustomer = @"
USE [$DatabaseName];
INSERT INTO [dbo].[Customers] ([Id], [Location])
VALUES ('$($customer.Id)', '$($customer.Location.Replace("'", "''"))');
"@
    sqlcmd -S $ServerName -E -Q $insertCustomer -b | Out-Null
}

# Insert Suppliers
Write-Host "Inserting suppliers..." -ForegroundColor Yellow
foreach ($supplier in $suppliers) {
    $insertSupplier = @"
USE [$DatabaseName];
INSERT INTO [dbo].[Suppliers] ([Id], [Location])
VALUES ('$($supplier.Id)', '$($supplier.Location.Replace("'", "''"))');
"@
    sqlcmd -S $ServerName -E -Q $insertSupplier -b | Out-Null
}

# Insert Orders
Write-Host "Inserting orders (this may take a moment)..." -ForegroundColor Yellow
$orderCount = 0
foreach ($row in $csvData) {
    $orderDate = [datetime]::Parse($row.'Order Date').ToString("yyyy-MM-dd HH:mm:ss.fff")
    $purchaseOrder = $row.'Purchase Order'
    $customerId = $row.'Customer ID'
    $supplierId = $row.'Fullfilled By ID'
    $orderReqAmt = $row.'Order Req Amt (Ton)'
    $suppliedAmt = $row.'Supplied Amt (Ton)'
    $costOfDelivery = $row.'Cost Of Delivery ($)'

    $insertOrder = @"
USE [$DatabaseName];
INSERT INTO [dbo].[GrainOrders]
    ([OrderDate], [PurchaseOrder], [CustomerId], [SupplierId], [OrderReqAmtTon], [SuppliedAmtTon], [CostOfDelivery])
VALUES
    ('$orderDate', '$purchaseOrder', '$customerId', '$supplierId', $orderReqAmt, $suppliedAmt, $costOfDelivery);
"@
    sqlcmd -S $ServerName -E -Q $insertOrder -b | Out-Null
    $orderCount++

    if ($orderCount % 20 -eq 0) {
        Write-Host "  Inserted $orderCount orders..." -ForegroundColor Gray
    }
}

Write-Host "`n=== Setup Complete ===" -ForegroundColor Green
Write-Host "Database: $DatabaseName" -ForegroundColor Cyan
Write-Host "Customers: $($customers.Count)" -ForegroundColor Cyan
Write-Host "Suppliers: $($suppliers.Count)" -ForegroundColor Cyan
Write-Host "Orders: $orderCount" -ForegroundColor Cyan
Write-Host "`nYou can now run your GrainBrokerAPI application!" -ForegroundColor Green
