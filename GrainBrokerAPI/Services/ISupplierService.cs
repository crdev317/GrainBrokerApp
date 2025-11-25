using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainBrokerAPI.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier?> GetSupplierByIdAsync(Guid id);
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task<bool> UpdateSupplierAsync(Guid id, Supplier supplier);
        Task<bool> DeleteSupplierAsync(Guid id);
        Task<bool> SupplierExistsAsync(Guid id);
    }
}
