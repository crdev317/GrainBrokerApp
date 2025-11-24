using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GrainBrokerAPITests
{
    public abstract class GrainBrokerControllerTestBase<TController, TEntity>
        where TController : ControllerBase
        where TEntity : class
    {
        protected GrainBrokerContext _context;
        protected TController _controller;

        protected abstract TController CreateController(GrainBrokerContext context);
        protected abstract TEntity CreateEntity(Guid id);
        protected abstract Guid GetEntityId(TEntity entity);
        protected abstract DbSet<TEntity> GetDbSet(GrainBrokerContext context);
        protected abstract string GetControllerName();

        [SetUp]
        public virtual void Setup()
        {
            var options = new DbContextOptionsBuilder<GrainBrokerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GrainBrokerContext(options);
            _controller = CreateController(_context);
        }

        [TearDown]
        public virtual void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task GetAll_ReturnsAllEntities()
        {
            // Arrange
            var entity1 = CreateEntity(Guid.NewGuid());
            var entity2 = CreateEntity(Guid.NewGuid());

            GetDbSet(_context).AddRange(entity1, entity2);
            await _context.SaveChangesAsync();

            // Act
            var result = await CallGetAllMethod();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<TEntity>>>());
            var entities = GetEntitiesFromResult(result);
            Assert.That(entities.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetById_WithValidId_ReturnsEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entity = CreateEntity(entityId);

            GetDbSet(_context).Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await CallGetByIdMethod(entityId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ActionResult<TEntity>>());
            var returnedEntity = GetEntityFromResult(result);
            Assert.That(returnedEntity, Is.Not.Null);
            Assert.That(GetEntityId(returnedEntity), Is.EqualTo(entityId));
        }

        [Test]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await CallGetByIdMethod(nonExistentId);

            // Assert
            var actionResult = result.Result;
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Post_CreatesNewEntity()
        {
            // Arrange
            var newEntity = CreateEntity(Guid.NewGuid());

            // Act
            var result = await CallPostMethod(newEntity);

            // Assert
            Assert.That(result, Is.Not.Null);
            var actionResult = result.Result;
            Assert.That(actionResult, Is.InstanceOf<CreatedAtActionResult>());

            var createdAtActionResult = actionResult as CreatedAtActionResult;
            var returnedEntity = createdAtActionResult.Value as TEntity;
            Assert.That(returnedEntity, Is.Not.Null);
            Assert.That(GetEntityId(returnedEntity), Is.EqualTo(GetEntityId(newEntity)));

            var dbEntity = await GetDbSet(_context).FindAsync(GetEntityId(newEntity));
            Assert.That(dbEntity, Is.Not.Null);
        }

        [Test]
        public async Task Put_WithValidId_UpdatesEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entity = CreateEntity(entityId);

            GetDbSet(_context).Add(entity);
            await _context.SaveChangesAsync();

            _context.Entry(entity).State = EntityState.Detached;

            var updatedEntity = CreateUpdatedEntity(entityId);

            // Act
            var result = await CallPutMethod(entityId, updatedEntity);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());

            var dbEntity = await GetDbSet(_context).FindAsync(entityId);
            Assert.That(dbEntity, Is.Not.Null);
            VerifyEntityUpdated(dbEntity);
        }

        [Test]
        public async Task Put_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var entity = CreateEntity(Guid.NewGuid());
            var differentId = Guid.NewGuid();

            // Act
            var result = await CallPutMethod(differentId, entity);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Delete_WithValidId_RemovesEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entity = CreateEntity(entityId);

            GetDbSet(_context).Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await CallDeleteMethod(entityId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());

            var dbEntity = await GetDbSet(_context).FindAsync(entityId);
            Assert.That(dbEntity, Is.Null);
        }

        [Test]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await CallDeleteMethod(nonExistentId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        protected abstract Task<ActionResult<IEnumerable<TEntity>>> CallGetAllMethod();
        protected abstract Task<ActionResult<TEntity>> CallGetByIdMethod(Guid id);
        protected abstract Task<ActionResult<TEntity>> CallPostMethod(TEntity entity);
        protected abstract Task<IActionResult> CallPutMethod(Guid id, TEntity entity);
        protected abstract Task<IActionResult> CallDeleteMethod(Guid id);
        protected abstract TEntity CreateUpdatedEntity(Guid id);
        protected abstract void VerifyEntityUpdated(TEntity entity);

        protected virtual IEnumerable<TEntity> GetEntitiesFromResult(ActionResult<IEnumerable<TEntity>> result)
        {
            return result.Value;
        }

        protected virtual TEntity GetEntityFromResult(ActionResult<TEntity> result)
        {
            return result.Value;
        }
    }
}
