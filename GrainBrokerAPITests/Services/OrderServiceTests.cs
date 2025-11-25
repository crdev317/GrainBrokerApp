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
    public class OrderServiceTests
    {
        private GrainBrokerContext _context;
        private OrderService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GrainBrokerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GrainBrokerContext(options);
            _service = new OrderService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task GetAllOrdersAsync_ReturnsAllOrders()
        {
            // Arrange
            var order1 = new Order { Id = Guid.NewGuid(), OrderReqAmtTon = 100, SuppliedAmtTon = 90, CostOfDelivery = 1000, CustomerId = Guid.NewGuid(), SupplierId = Guid.NewGuid() };
            var order2 = new Order { Id = Guid.NewGuid(), OrderReqAmtTon = 200, SuppliedAmtTon = 180, CostOfDelivery = 2000, CustomerId = Guid.NewGuid(), SupplierId = Guid.NewGuid() };
            _context.GrainOrders.AddRange(order1, order2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllOrdersAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetOrderByIdAsync_WithValidId_ReturnsOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, OrderReqAmtTon = 100, SuppliedAmtTon = 90, CostOfDelivery = 1000, CustomerId = Guid.NewGuid(), SupplierId = Guid.NewGuid() };
            _context.GrainOrders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetOrderByIdAsync(orderId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(orderId));
            Assert.That(result.OrderReqAmtTon, Is.EqualTo(100));
        }

        [Test]
        public async Task GetOrderByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.GetOrderByIdAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateOrderAsync_WithValidOrder_CreatesOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderReqAmtTon = 100,
                SuppliedAmtTon = 90,
                CostOfDelivery = 1000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };

            // Act
            var result = await _service.CreateOrderAsync(order);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(order.Id));

            var dbOrder = await _context.GrainOrders.FindAsync(order.Id);
            Assert.That(dbOrder, Is.Not.Null);
            Assert.That(dbOrder.OrderReqAmtTon, Is.EqualTo(100));
        }

        [Test]
        public void CreateOrderAsync_WithNullOrder_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.CreateOrderAsync(null));
        }

        [Test]
        public void CreateOrderAsync_WithZeroOrderAmount_ThrowsArgumentException()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderReqAmtTon = 0,
                SuppliedAmtTon = 90,
                CostOfDelivery = 1000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.CreateOrderAsync(order));
            Assert.That(ex.Message, Does.Contain("Order request amount must be greater than zero"));
        }

        [Test]
        public void CreateOrderAsync_WithNegativeOrderAmount_ThrowsArgumentException()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderReqAmtTon = -10,
                SuppliedAmtTon = 90,
                CostOfDelivery = 1000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.CreateOrderAsync(order));
            Assert.That(ex.Message, Does.Contain("Order request amount must be greater than zero"));
        }

        [Test]
        public void CreateOrderAsync_WithNegativeSuppliedAmount_ThrowsArgumentException()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderReqAmtTon = 100,
                SuppliedAmtTon = -10,
                CostOfDelivery = 1000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.CreateOrderAsync(order));
            Assert.That(ex.Message, Does.Contain("Supplied amount cannot be negative"));
        }

        [Test]
        public void CreateOrderAsync_WithNegativeCost_ThrowsArgumentException()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderReqAmtTon = 100,
                SuppliedAmtTon = 90,
                CostOfDelivery = -1000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.CreateOrderAsync(order));
            Assert.That(ex.Message, Does.Contain("Cost of delivery cannot be negative"));
        }

        [Test]
        public async Task UpdateOrderAsync_WithValidOrder_UpdatesOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                OrderReqAmtTon = 100,
                SuppliedAmtTon = 90,
                CostOfDelivery = 1000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };
            _context.GrainOrders.Add(order);
            await _context.SaveChangesAsync();

            _context.Entry(order).State = EntityState.Detached;

            var updatedOrder = new Order
            {
                Id = orderId,
                OrderReqAmtTon = 200,
                SuppliedAmtTon = 180,
                CostOfDelivery = 2000,
                CustomerId = order.CustomerId,
                SupplierId = order.SupplierId
            };

            // Act
            var result = await _service.UpdateOrderAsync(orderId, updatedOrder);

            // Assert
            Assert.That(result, Is.True);
            var dbOrder = await _context.GrainOrders.FindAsync(orderId);
            Assert.That(dbOrder.OrderReqAmtTon, Is.EqualTo(200));
            Assert.That(dbOrder.SuppliedAmtTon, Is.EqualTo(180));
            Assert.That(dbOrder.CostOfDelivery, Is.EqualTo(2000));
        }

        [Test]
        public async Task UpdateOrderAsync_WithMismatchedId_ReturnsFalse()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderReqAmtTon = 100,
                SuppliedAmtTon = 90,
                CostOfDelivery = 1000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };
            var differentId = Guid.NewGuid();

            // Act
            var result = await _service.UpdateOrderAsync(differentId, order);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteOrderAsync_WithValidId_DeletesOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                OrderReqAmtTon = 100,
                SuppliedAmtTon = 90,
                CostOfDelivery = 1000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };
            _context.GrainOrders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteOrderAsync(orderId);

            // Assert
            Assert.That(result, Is.True);
            var dbOrder = await _context.GrainOrders.FindAsync(orderId);
            Assert.That(dbOrder, Is.Null);
        }

        [Test]
        public async Task DeleteOrderAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.DeleteOrderAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task OrderExistsAsync_WithExistingOrder_ReturnsTrue()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                OrderReqAmtTon = 100,
                SuppliedAmtTon = 90,
                CostOfDelivery = 1000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };
            _context.GrainOrders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.OrderExistsAsync(orderId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task OrderExistsAsync_WithNonExistentOrder_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.OrderExistsAsync(nonExistentId);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
