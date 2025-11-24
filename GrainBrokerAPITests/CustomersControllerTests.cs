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
    public class CustomersControllerTests : GrainBrokerControllerTestBase<CustomersController, Customer>
    {
        protected override CustomersController CreateController(GrainBrokerContext context)
        {
            return new CustomersController(context);
        }

        protected override Customer CreateEntity(Guid id)
        {
            return new Customer
            {
                Id = id,
                Location = "Original Location"
            };
        }

        protected override Customer CreateUpdatedEntity(Guid id)
        {
            return new Customer
            {
                Id = id,
                Location = "Updated Location"
            };
        }

        protected override Guid GetEntityId(Customer entity)
        {
            return entity.Id;
        }

        protected override DbSet<Customer> GetDbSet(GrainBrokerContext context)
        {
            return context.Customers;
        }

        protected override string GetControllerName()
        {
            return "Customers";
        }

        protected override Task<ActionResult<IEnumerable<Customer>>> CallGetAllMethod()
        {
            return _controller.GetCustomers();
        }

        protected override Task<ActionResult<Customer>> CallGetByIdMethod(Guid id)
        {
            return _controller.GetCustomer(id);
        }

        protected override Task<ActionResult<Customer>> CallPostMethod(Customer entity)
        {
            return _controller.PostCustomer(entity);
        }

        protected override Task<IActionResult> CallPutMethod(Guid id, Customer entity)
        {
            return _controller.PutCustomer(id, entity);
        }

        protected override Task<IActionResult> CallDeleteMethod(Guid id)
        {
            return _controller.DeleteCustomer(id);
        }

        protected override void VerifyEntityUpdated(Customer entity)
        {
            Assert.That(entity.Location, Is.EqualTo("Updated Location"));
        }
    }
}
