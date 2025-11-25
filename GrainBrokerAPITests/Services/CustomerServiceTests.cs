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
    public class CustomerServiceTests
    {
        private GrainBrokerContext _context;
        private CustomerService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GrainBrokerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GrainBrokerContext(options);
            _service = new CustomerService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task GetAllCustomersAsync_ReturnsAllCustomers()
        {
            // Arrange
            var customer1 = new Customer { Id = Guid.NewGuid(), Location = "Location1" };
            var customer2 = new Customer { Id = Guid.NewGuid(), Location = "Location2" };
            _context.Customers.AddRange(customer1, customer2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllCustomersAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetCustomerByIdAsync_WithValidId_ReturnsCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, Location = "Test Location" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(customerId));
            Assert.That(result.Location, Is.EqualTo("Test Location"));
        }

        [Test]
        public async Task GetCustomerByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.GetCustomerByIdAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateCustomerAsync_WithValidCustomer_CreatesCustomer()
        {
            // Arrange
            var customer = new Customer { Id = Guid.NewGuid(), Location = "New Location" };

            // Act
            var result = await _service.CreateCustomerAsync(customer);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(customer.Id));

            var dbCustomer = await _context.Customers.FindAsync(customer.Id);
            Assert.That(dbCustomer, Is.Not.Null);
            Assert.That(dbCustomer.Location, Is.EqualTo("New Location"));
        }

        [Test]
        public void CreateCustomerAsync_WithNullCustomer_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.CreateCustomerAsync(null));
        }

        [Test]
        public void CreateCustomerAsync_WithEmptyLocation_ThrowsArgumentException()
        {
            // Arrange
            var customer = new Customer { Id = Guid.NewGuid(), Location = "" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.CreateCustomerAsync(customer));
            Assert.That(ex.Message, Does.Contain("Location is required"));
        }

        [Test]
        public void CreateCustomerAsync_WithNullLocation_ThrowsArgumentException()
        {
            // Arrange
            var customer = new Customer { Id = Guid.NewGuid(), Location = null };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.CreateCustomerAsync(customer));
            Assert.That(ex.Message, Does.Contain("Location is required"));
        }

        [Test]
        public async Task UpdateCustomerAsync_WithValidCustomer_UpdatesCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, Location = "Original Location" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _context.Entry(customer).State = EntityState.Detached;

            var updatedCustomer = new Customer { Id = customerId, Location = "Updated Location" };

            // Act
            var result = await _service.UpdateCustomerAsync(customerId, updatedCustomer);

            // Assert
            Assert.That(result, Is.True);
            var dbCustomer = await _context.Customers.FindAsync(customerId);
            Assert.That(dbCustomer.Location, Is.EqualTo("Updated Location"));
        }

        [Test]
        public async Task UpdateCustomerAsync_WithMismatchedId_ReturnsFalse()
        {
            // Arrange
            var customer = new Customer { Id = Guid.NewGuid(), Location = "Test" };
            var differentId = Guid.NewGuid();

            // Act
            var result = await _service.UpdateCustomerAsync(differentId, customer);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateCustomerAsync_WithNonExistentCustomer_ThrowsConcurrencyException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, Location = "Test" };

            // Mark entity as modified without it being in the database
            _context.Customers.Attach(customer);
            _context.Entry(customer).State = EntityState.Modified;

            // Act & Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
                await _context.SaveChangesAsync());
        }

        [Test]
        public async Task DeleteCustomerAsync_WithValidId_DeletesCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, Location = "Test Location" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteCustomerAsync(customerId);

            // Assert
            Assert.That(result, Is.True);
            var dbCustomer = await _context.Customers.FindAsync(customerId);
            Assert.That(dbCustomer, Is.Null);
        }

        [Test]
        public async Task DeleteCustomerAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.DeleteCustomerAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CustomerExistsAsync_WithExistingCustomer_ReturnsTrue()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, Location = "Test" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CustomerExistsAsync(customerId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CustomerExistsAsync_WithNonExistentCustomer_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.CustomerExistsAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
