using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GrainBrokerAPI.Services
{
    /// <summary>
    /// Service implementation for Supplier business logic
    /// </summary>
    public class SupplierService : ISupplierService
    {
        private readonly GrainBrokerContext _context;

        public SupplierService(GrainBrokerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(Guid id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            if (string.IsNullOrWhiteSpace(supplier.Location))
                throw new ArgumentException("Location is required", nameof(supplier));

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return supplier;
        }

        public async Task<bool> UpdateSupplierAsync(Guid id, Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            if (id != supplier.Id)
                return false;

            _context.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SupplierExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteSupplierAsync(Guid id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
                return false;

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SupplierExistsAsync(Guid id)
        {
            return await _context.Suppliers.AnyAsync(e => e.Id == id);
        }
    }
}
