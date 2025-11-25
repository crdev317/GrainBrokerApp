using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainBrokerAPI.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(Guid id);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Guid id, Customer customer);
        Task<bool> DeleteCustomerAsync(Guid id);
        Task<bool> CustomerExistsAsync(Guid id);
    }
}
