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
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(Guid id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates an existing supplier
        /// </summary>
        /// <remarks>
        /// Minimum required data:
        ///
        ///     PUT /api/Suppliers/{id}
        ///     {
        ///         "id": "c3d4e5f6-7890-4ef0-d5fe-34567890abcd",
        ///         "location": "Updated Location"
        ///     }
        ///
        /// Note: The id in the URL must match the id in the request body
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(Guid id, Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var success = await _supplierService.UpdateSupplierAsync(id, supplier);

                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _supplierService.SupplierExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new supplier
        /// </summary>
        /// <remarks>
        /// Minimum required data:
        ///
        ///     POST /api/Suppliers
        ///     {
        ///         "location": "Iowa Farm Co-op"
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
        {
            try
            {
                var createdSupplier = await _supplierService.CreateSupplierAsync(supplier);
                return CreatedAtAction("GetSupplier", new { id = createdSupplier.Id }, createdSupplier);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            var success = await _supplierService.DeleteSupplierAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
