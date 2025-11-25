using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using GrainBrokerAPI.Controllers;
using GrainBrokerAPI.Services;
using NUnit.Framework;

namespace GrainBrokerAPITests
{
    [TestFixture]
    public class OrdersControllerTests
    {
        private Mock<IOrderService> _mockService;
        private OrdersController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IOrderService>();
            _controller = new OrdersController(_mockService.Object);
        }

        [Test]
        public async Task GetGrainOrders_ReturnsOkWithAllOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), OrderReqAmtTon = 100, SuppliedAmtTon = 90, CostOfDelivery = 1000 },
                new Order { Id = Guid.NewGuid(), OrderReqAmtTon = 200, SuppliedAmtTon = 180, CostOfDelivery = 2000 }
            };
            _mockService.Setup(s => s.GetAllOrdersAsync()).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetGrainOrders();

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedOrders = okResult.Value as IEnumerable<Order>;
            Assert.That(returnedOrders, Is.Not.Null);
            _mockService.Verify(s => s.GetAllOrdersAsync(), Times.Once);
        }

        [Test]
        public async Task GetOrder_WithValidId_ReturnsOkWithOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, OrderReqAmtTon = 100, SuppliedAmtTon = 90, CostOfDelivery = 1000 };
            _mockService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedOrder = okResult.Value as Order;
            Assert.That(returnedOrder, Is.Not.Null);
            Assert.That(returnedOrder.Id, Is.EqualTo(orderId));
            _mockService.Verify(s => s.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Test]
        public async Task GetOrder_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _mockService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Test]
        public async Task PostOrder_WithValidOrder_ReturnsCreatedAtAction()
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
            _mockService.Setup(s => s.CreateOrderAsync(order)).ReturnsAsync(order);

            // Act
            var result = await _controller.PostOrder(order);

            // Assert
            Assert.That(result, Is.Not.Null);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.ActionName, Is.EqualTo("GetOrder"));
            var returnedOrder = createdResult.Value as Order;
            Assert.That(returnedOrder, Is.Not.Null);
            Assert.That(returnedOrder.Id, Is.EqualTo(order.Id));
            _mockService.Verify(s => s.CreateOrderAsync(order), Times.Once);
        }

        [Test]
        public async Task PostOrder_WithInvalidOrder_ReturnsBadRequest()
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
            _mockService.Setup(s => s.CreateOrderAsync(order))
                .ThrowsAsync(new ArgumentException("Order request amount must be greater than zero"));

            // Act
            var result = await _controller.PostOrder(order);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            _mockService.Verify(s => s.CreateOrderAsync(order), Times.Once);
        }

        [Test]
        public async Task PutOrder_WithValidOrder_ReturnsNoContent()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                OrderReqAmtTon = 200,
                SuppliedAmtTon = 180,
                CostOfDelivery = 2000,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };
            _mockService.Setup(s => s.UpdateOrderAsync(orderId, order)).ReturnsAsync(true);

            // Act
            var result = await _controller.PutOrder(orderId, order);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _mockService.Verify(s => s.UpdateOrderAsync(orderId, order), Times.Once);
        }

        [Test]
        public async Task PutOrder_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var order = new Order { Id = Guid.NewGuid(), OrderReqAmtTon = 100, SuppliedAmtTon = 90, CostOfDelivery = 1000 };
            var differentId = Guid.NewGuid();

            // Act
            var result = await _controller.PutOrder(differentId, order);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            _mockService.Verify(s => s.UpdateOrderAsync(It.IsAny<Guid>(), It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public async Task PutOrder_WithNonExistentOrder_ReturnsNotFound()
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
            _mockService.Setup(s => s.UpdateOrderAsync(orderId, order)).ReturnsAsync(false);

            // Act
            var result = await _controller.PutOrder(orderId, order);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.UpdateOrderAsync(orderId, order), Times.Once);
        }

        [Test]
        public async Task DeleteOrder_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteOrderAsync(orderId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _mockService.Verify(s => s.DeleteOrderAsync(orderId), Times.Once);
        }

        [Test]
        public async Task DeleteOrder_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteOrderAsync(orderId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.DeleteOrderAsync(orderId), Times.Once);
        }
    }
}
