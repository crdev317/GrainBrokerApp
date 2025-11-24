using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrainBrokerAPI.Controllers;
using NUnit.Framework;

namespace GrainBrokerAPITests
{
    [TestFixture]
    public class SuppliersControllerTests : GrainBrokerControllerTestBase<SuppliersController, Supplier>
    {
        protected override SuppliersController CreateController(GrainBrokerContext context)
        {
            return new SuppliersController(context);
        }

        protected override Supplier CreateEntity(Guid id)
        {
            return new Supplier
            {
                Id = id,
                Location = "Original Location"
            };
        }

        protected override Supplier CreateUpdatedEntity(Guid id)
        {
            return new Supplier
            {
                Id = id,
                Location = "Updated Location"
            };
        }

        protected override Guid GetEntityId(Supplier entity)
        {
            return entity.Id;
        }

        protected override DbSet<Supplier> GetDbSet(GrainBrokerContext context)
        {
            return context.Suppliers;
        }

        protected override string GetControllerName()
        {
            return "Suppliers";
        }

        protected override Task<ActionResult<IEnumerable<Supplier>>> CallGetAllMethod()
        {
            return _controller.GetSuppliers();
        }

        protected override Task<ActionResult<Supplier>> CallGetByIdMethod(Guid id)
        {
            return _controller.GetSupplier(id);
        }

        protected override Task<ActionResult<Supplier>> CallPostMethod(Supplier entity)
        {
            return _controller.PostSupplier(entity);
        }

        protected override Task<IActionResult> CallPutMethod(Guid id, Supplier entity)
        {
            return _controller.PutSupplier(id, entity);
        }

        protected override Task<IActionResult> CallDeleteMethod(Guid id)
        {
            return _controller.DeleteSupplier(id);
        }

        protected override void VerifyEntityUpdated(Supplier entity)
        {
            Assert.That(entity.Location, Is.EqualTo("Updated Location"));
        }
    }
}
