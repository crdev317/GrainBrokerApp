using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using GrainBrokerAPI.Services;

namespace GrainBrokerAPITests.Services
{
    [TestFixture]
    public class SupplierServiceTests
    {
        private GrainBrokerContext _context;
        private SupplierService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GrainBrokerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GrainBrokerContext(options);
            _service = new SupplierService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task GetAllSuppliersAsync_ReturnsAllSuppliers()
        {
            // Arrange
            var supplier1 = new Supplier { Id = Guid.NewGuid(), Location = "Location1" };
            var supplier2 = new Supplier { Id = Guid.NewGuid(), Location = "Location2" };
            _context.Suppliers.AddRange(supplier1, supplier2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllSuppliersAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetSupplierByIdAsync_WithValidId_ReturnsSupplier()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            var supplier = new Supplier { Id = supplierId, Location = "Test Location" };
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetSupplierByIdAsync(supplierId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(supplierId));
            Assert.That(result.Location, Is.EqualTo("Test Location"));
        }

        [Test]
        public async Task GetSupplierByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.GetSupplierByIdAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateSupplierAsync_WithValidSupplier_CreatesSupplier()
        {
            // Arrange
            var supplier = new Supplier { Id = Guid.NewGuid(), Location = "New Location" };

            // Act
            var result = await _service.CreateSupplierAsync(supplier);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(supplier.Id));

            var dbSupplier = await _context.Suppliers.FindAsync(supplier.Id);
            Assert.That(dbSupplier, Is.Not.Null);
            Assert.That(dbSupplier.Location, Is.EqualTo("New Location"));
        }

        [Test]
        public void CreateSupplierAsync_WithNullSupplier_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.CreateSupplierAsync(null));
        }

        [Test]
        public void CreateSupplierAsync_WithEmptyLocation_ThrowsArgumentException()
        {
            // Arrange
            var supplier = new Supplier { Id = Guid.NewGuid(), Location = "" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.CreateSupplierAsync(supplier));
            Assert.That(ex.Message, Does.Contain("Location is required"));
        }

        [Test]
        public void CreateSupplierAsync_WithNullLocation_ThrowsArgumentException()
        {
            // Arrange
            var supplier = new Supplier { Id = Guid.NewGuid(), Location = null };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.CreateSupplierAsync(supplier));
            Assert.That(ex.Message, Does.Contain("Location is required"));
        }

        [Test]
        public async Task UpdateSupplierAsync_WithValidSupplier_UpdatesSupplier()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            var supplier = new Supplier { Id = supplierId, Location = "Original Location" };
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            _context.Entry(supplier).State = EntityState.Detached;

            var updatedSupplier = new Supplier { Id = supplierId, Location = "Updated Location" };

            // Act
            var result = await _service.UpdateSupplierAsync(supplierId, updatedSupplier);

            // Assert
            Assert.That(result, Is.True);
            var dbSupplier = await _context.Suppliers.FindAsync(supplierId);
            Assert.That(dbSupplier.Location, Is.EqualTo("Updated Location"));
        }

        [Test]
        public async Task UpdateSupplierAsync_WithMismatchedId_ReturnsFalse()
        {
            // Arrange
            var supplier = new Supplier { Id = Guid.NewGuid(), Location = "Test" };
            var differentId = Guid.NewGuid();

            // Act
            var result = await _service.UpdateSupplierAsync(differentId, supplier);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateSupplierAsync_WithNonExistentSupplier_ThrowsConcurrencyException()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            var supplier = new Supplier { Id = supplierId, Location = "Test" };

            // Mark entity as modified without it being in the database
            _context.Suppliers.Attach(supplier);
            _context.Entry(supplier).State = EntityState.Modified;

            // Act & Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
                await _context.SaveChangesAsync());
        }

        [Test]
        public async Task DeleteSupplierAsync_WithValidId_DeletesSupplier()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            var supplier = new Supplier { Id = supplierId, Location = "Test Location" };
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteSupplierAsync(supplierId);

            // Assert
            Assert.That(result, Is.True);
            var dbSupplier = await _context.Suppliers.FindAsync(supplierId);
            Assert.That(dbSupplier, Is.Null);
        }

        [Test]
        public async Task DeleteSupplierAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.DeleteSupplierAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SupplierExistsAsync_WithExistingSupplier_ReturnsTrue()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            var supplier = new Supplier { Id = supplierId, Location = "Test" };
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.SupplierExistsAsync(supplierId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task SupplierExistsAsync_WithNonExistentSupplier_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.SupplierExistsAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
