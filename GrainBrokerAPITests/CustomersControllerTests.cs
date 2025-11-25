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
    public class CustomersControllerTests
    {
        private Mock<ICustomerService> _mockService;
        private CustomersController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<ICustomerService>();
            _controller = new CustomersController(_mockService.Object);
        }

        [Test]
        public async Task GetCustomers_ReturnsOkWithAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = Guid.NewGuid(), Location = "Location1" },
                new Customer { Id = Guid.NewGuid(), Location = "Location2" }
            };
            _mockService.Setup(s => s.GetAllCustomersAsync()).ReturnsAsync(customers);

            // Act
            var result = await _controller.GetCustomers();

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedCustomers = okResult.Value as IEnumerable<Customer>;
            Assert.That(returnedCustomers, Is.Not.Null);
            _mockService.Verify(s => s.GetAllCustomersAsync(), Times.Once);
        }

        [Test]
        public async Task GetCustomer_WithValidId_ReturnsOkWithCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, Location = "Test Location" };
            _mockService.Setup(s => s.GetCustomerByIdAsync(customerId)).ReturnsAsync(customer);

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedCustomer = okResult.Value as Customer;
            Assert.That(returnedCustomer, Is.Not.Null);
            Assert.That(returnedCustomer.Id, Is.EqualTo(customerId));
            _mockService.Verify(s => s.GetCustomerByIdAsync(customerId), Times.Once);
        }

        [Test]
        public async Task GetCustomer_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _mockService.Setup(s => s.GetCustomerByIdAsync(customerId)).ReturnsAsync((Customer)null);

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.GetCustomerByIdAsync(customerId), Times.Once);
        }

        [Test]
        public async Task PostCustomer_WithValidCustomer_ReturnsCreatedAtAction()
        {
            // Arrange
            var customer = new Customer { Id = Guid.NewGuid(), Location = "New Location" };
            _mockService.Setup(s => s.CreateCustomerAsync(customer)).ReturnsAsync(customer);

            // Act
            var result = await _controller.PostCustomer(customer);

            // Assert
            Assert.That(result, Is.Not.Null);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.ActionName, Is.EqualTo("GetCustomer"));
            var returnedCustomer = createdResult.Value as Customer;
            Assert.That(returnedCustomer, Is.Not.Null);
            Assert.That(returnedCustomer.Id, Is.EqualTo(customer.Id));
            _mockService.Verify(s => s.CreateCustomerAsync(customer), Times.Once);
        }

        [Test]
        public async Task PostCustomer_WithInvalidCustomer_ReturnsBadRequest()
        {
            // Arrange
            var customer = new Customer { Id = Guid.NewGuid(), Location = "" };
            _mockService.Setup(s => s.CreateCustomerAsync(customer))
                .ThrowsAsync(new ArgumentException("Location is required"));

            // Act
            var result = await _controller.PostCustomer(customer);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            _mockService.Verify(s => s.CreateCustomerAsync(customer), Times.Once);
        }

        [Test]
        public async Task PutCustomer_WithValidCustomer_ReturnsNoContent()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, Location = "Updated Location" };
            _mockService.Setup(s => s.UpdateCustomerAsync(customerId, customer)).ReturnsAsync(true);

            // Act
            var result = await _controller.PutCustomer(customerId, customer);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _mockService.Verify(s => s.UpdateCustomerAsync(customerId, customer), Times.Once);
        }

        [Test]
        public async Task PutCustomer_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var customer = new Customer { Id = Guid.NewGuid(), Location = "Test" };
            var differentId = Guid.NewGuid();

            // Act
            var result = await _controller.PutCustomer(differentId, customer);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            _mockService.Verify(s => s.UpdateCustomerAsync(It.IsAny<Guid>(), It.IsAny<Customer>()), Times.Never);
        }

        [Test]
        public async Task PutCustomer_WithNonExistentCustomer_ReturnsNotFound()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId, Location = "Test" };
            _mockService.Setup(s => s.UpdateCustomerAsync(customerId, customer)).ReturnsAsync(false);

            // Act
            var result = await _controller.PutCustomer(customerId, customer);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.UpdateCustomerAsync(customerId, customer), Times.Once);
        }

        [Test]
        public async Task DeleteCustomer_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteCustomerAsync(customerId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _mockService.Verify(s => s.DeleteCustomerAsync(customerId), Times.Once);
        }

        [Test]
        public async Task DeleteCustomer_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteCustomerAsync(customerId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.DeleteCustomerAsync(customerId), Times.Once);
        }
    }
}
