using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrainBrokerAPI.Services;

namespace GrainBrokerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetGrainOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates an existing order
        /// </summary>
        /// <remarks>
        /// Minimum required data:
        ///
        ///     PUT /api/Orders/{id}
        ///     {
        ///         "id": "a1b2c3d4-5678-4abc-b3fc-1234567890ab",
        ///         "orderDate": "10:30:00",
        ///         "purchaseOrder": "b2c3d4e5-6789-4def-c4fd-234567890abc",
        ///         "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "supplierId": "c3d4e5f6-7890-4ef0-d5fe-34567890abcd",
        ///         "orderReqAmtTon": 150,
        ///         "suppliedAmtTon": 150,
        ///         "costOfDelivery": 6000.00
        ///     }
        ///
        /// Note: The id in the URL must match the id in the request body
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(Guid id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var success = await _orderService.UpdateOrderAsync(id, order);

                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _orderService.OrderExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <remarks>
        /// Minimum required data (customerId and supplierId must reference existing entities):
        ///
        ///     POST /api/Orders
        ///     {
        ///         "orderDate": "10:30:00",
        ///         "purchaseOrder": "b2c3d4e5-6789-4def-c4fd-234567890abc",
        ///         "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "supplierId": "c3d4e5f6-7890-4ef0-d5fe-34567890abcd",
        ///         "orderReqAmtTon": 100,
        ///         "suppliedAmtTon": 95,
        ///         "costOfDelivery": 5000.00
        ///     }
        ///
        /// Note: orderDate is TimeSpan format (HH:mm:ss or d.HH:mm:ss)
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(order);
                return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var success = await _orderService.DeleteOrderAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
