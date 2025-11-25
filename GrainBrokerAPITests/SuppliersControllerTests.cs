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
    public class SuppliersControllerTests
    {
        private Mock<ISupplierService> _mockService;
        private SuppliersController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<ISupplierService>();
            _controller = new SuppliersController(_mockService.Object);
        }

        [Test]
        public async Task GetSuppliers_ReturnsOkWithAllSuppliers()
        {
            // Arrange
            var suppliers = new List<Supplier>
            {
                new Supplier { Id = Guid.NewGuid(), Location = "Location1" },
                new Supplier { Id = Guid.NewGuid(), Location = "Location2" }
            };
            _mockService.Setup(s => s.GetAllSuppliersAsync()).ReturnsAsync(suppliers);

            // Act
            var result = await _controller.GetSuppliers();

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedSuppliers = okResult.Value as IEnumerable<Supplier>;
            Assert.That(returnedSuppliers, Is.Not.Null);
            _mockService.Verify(s => s.GetAllSuppliersAsync(), Times.Once);
        }

        [Test]
        public async Task GetSupplier_WithValidId_ReturnsOkWithSupplier()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            var supplier = new Supplier { Id = supplierId, Location = "Test Location" };
            _mockService.Setup(s => s.GetSupplierByIdAsync(supplierId)).ReturnsAsync(supplier);

            // Act
            var result = await _controller.GetSupplier(supplierId);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedSupplier = okResult.Value as Supplier;
            Assert.That(returnedSupplier, Is.Not.Null);
            Assert.That(returnedSupplier.Id, Is.EqualTo(supplierId));
            _mockService.Verify(s => s.GetSupplierByIdAsync(supplierId), Times.Once);
        }

        [Test]
        public async Task GetSupplier_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            _mockService.Setup(s => s.GetSupplierByIdAsync(supplierId)).ReturnsAsync((Supplier)null);

            // Act
            var result = await _controller.GetSupplier(supplierId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.GetSupplierByIdAsync(supplierId), Times.Once);
        }

        [Test]
        public async Task PostSupplier_WithValidSupplier_ReturnsCreatedAtAction()
        {
            // Arrange
            var supplier = new Supplier { Id = Guid.NewGuid(), Location = "New Location" };
            _mockService.Setup(s => s.CreateSupplierAsync(supplier)).ReturnsAsync(supplier);

            // Act
            var result = await _controller.PostSupplier(supplier);

            // Assert
            Assert.That(result, Is.Not.Null);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.ActionName, Is.EqualTo("GetSupplier"));
            var returnedSupplier = createdResult.Value as Supplier;
            Assert.That(returnedSupplier, Is.Not.Null);
            Assert.That(returnedSupplier.Id, Is.EqualTo(supplier.Id));
            _mockService.Verify(s => s.CreateSupplierAsync(supplier), Times.Once);
        }

        [Test]
        public async Task PostSupplier_WithInvalidSupplier_ReturnsBadRequest()
        {
            // Arrange
            var supplier = new Supplier { Id = Guid.NewGuid(), Location = "" };
            _mockService.Setup(s => s.CreateSupplierAsync(supplier))
                .ThrowsAsync(new ArgumentException("Location is required"));

            // Act
            var result = await _controller.PostSupplier(supplier);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            _mockService.Verify(s => s.CreateSupplierAsync(supplier), Times.Once);
        }

        [Test]
        public async Task PutSupplier_WithValidSupplier_ReturnsNoContent()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            var supplier = new Supplier { Id = supplierId, Location = "Updated Location" };
            _mockService.Setup(s => s.UpdateSupplierAsync(supplierId, supplier)).ReturnsAsync(true);

            // Act
            var result = await _controller.PutSupplier(supplierId, supplier);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _mockService.Verify(s => s.UpdateSupplierAsync(supplierId, supplier), Times.Once);
        }

        [Test]
        public async Task PutSupplier_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var supplier = new Supplier { Id = Guid.NewGuid(), Location = "Test" };
            var differentId = Guid.NewGuid();

            // Act
            var result = await _controller.PutSupplier(differentId, supplier);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            _mockService.Verify(s => s.UpdateSupplierAsync(It.IsAny<Guid>(), It.IsAny<Supplier>()), Times.Never);
        }

        [Test]
        public async Task PutSupplier_WithNonExistentSupplier_ReturnsNotFound()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            var supplier = new Supplier { Id = supplierId, Location = "Test" };
            _mockService.Setup(s => s.UpdateSupplierAsync(supplierId, supplier)).ReturnsAsync(false);

            // Act
            var result = await _controller.PutSupplier(supplierId, supplier);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.UpdateSupplierAsync(supplierId, supplier), Times.Once);
        }

        [Test]
        public async Task DeleteSupplier_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteSupplierAsync(supplierId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteSupplier(supplierId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _mockService.Verify(s => s.DeleteSupplierAsync(supplierId), Times.Once);
        }

        [Test]
        public async Task DeleteSupplier_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteSupplierAsync(supplierId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteSupplier(supplierId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.DeleteSupplierAsync(supplierId), Times.Once);
        }
    }
}
