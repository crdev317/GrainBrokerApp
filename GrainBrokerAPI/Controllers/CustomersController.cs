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
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates an existing customer
        /// </summary>
        /// <remarks>
        /// Minimum required data:
        ///
        ///     PUT /api/Customers/{id}
        ///     {
        ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "location": "Updated Location"
        ///     }
        ///
        /// Note: The id in the URL must match the id in the request body
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(Guid id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var success = await _customerService.UpdateCustomerAsync(id, customer);

                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _customerService.CustomerExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new customer with optional orders
        /// </summary>
        /// <remarks>
        /// Minimum required data:
        ///
        ///     POST /api/Customers
        ///     {
        ///         "location": "Chicago, IL"
        ///     }
        ///
        /// With orders (supplierId must reference an existing supplier):
        ///
        ///     POST /api/Customers
        ///     {
        ///         "location": "Chicago, IL",
        ///         "orders": [
        ///             {
        ///                 "orderDate": "10:30:00",
        ///                 "purchaseOrder": "b2c3d4e5-6789-4def-c4fd-234567890abc",
        ///                 "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///                 "supplierId": "c3d4e5f6-7890-4ef0-d5fe-34567890abcd",
        ///                 "orderReqAmtTon": 100,
        ///                 "suppliedAmtTon": 95,
        ///                 "costOfDelivery": 5000.00
        ///             }
        ///         ]
        ///     }
        ///
        /// Note: orderDate is TimeSpan format (HH:mm:ss or d.HH:mm:ss)
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            try
            {
                var createdCustomer = await _customerService.CreateCustomerAsync(customer);
                return CreatedAtAction("GetCustomer", new { id = createdCustomer.Id }, createdCustomer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var success = await _customerService.DeleteCustomerAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
