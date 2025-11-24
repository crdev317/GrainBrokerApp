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
    public class OrdersControllerTests : GrainBrokerControllerTestBase<OrdersController, Order>
    {
        protected override OrdersController CreateController(GrainBrokerContext context)
        {
            return new OrdersController(context);
        }

        protected override Order CreateEntity(Guid id)
        {
            return new Order
            {
                Id = id,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid(),
                OrderReqAmtTon = 100,
                SuppliedAmtTon = 0,
                CostOfDelivery = 1000.00m
            };
        }

        protected override Order CreateUpdatedEntity(Guid id)
        {
            return new Order
            {
                Id = id,
                CustomerId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid(),
                OrderReqAmtTon = 200,
                SuppliedAmtTon = 150,
                CostOfDelivery = 2000.00m
            };
        }

        protected override Guid GetEntityId(Order entity)
        {
            return entity.Id;
        }

        protected override DbSet<Order> GetDbSet(GrainBrokerContext context)
        {
            return context.GrainOrders;
        }

        protected override string GetControllerName()
        {
            return "Orders";
        }

        protected override Task<ActionResult<IEnumerable<Order>>> CallGetAllMethod()
        {
            return _controller.GetGrainOrders();
        }

        protected override Task<ActionResult<Order>> CallGetByIdMethod(Guid id)
        {
            return _controller.GetOrder(id);
        }

        protected override Task<ActionResult<Order>> CallPostMethod(Order entity)
        {
            return _controller.PostOrder(entity);
        }

        protected override Task<IActionResult> CallPutMethod(Guid id, Order entity)
        {
            return _controller.PutOrder(id, entity);
        }

        protected override Task<IActionResult> CallDeleteMethod(Guid id)
        {
            return _controller.DeleteOrder(id);
        }

        protected override void VerifyEntityUpdated(Order entity)
        {
            Assert.That(entity.OrderReqAmtTon, Is.EqualTo(200));
            Assert.That(entity.SuppliedAmtTon, Is.EqualTo(150));
            Assert.That(entity.CostOfDelivery, Is.EqualTo(2000.00m));
        }
    }
}
