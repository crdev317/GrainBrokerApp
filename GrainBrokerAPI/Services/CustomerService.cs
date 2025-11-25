using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GrainBrokerAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly GrainBrokerContext _context;

        public CustomerService(GrainBrokerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (string.IsNullOrWhiteSpace(customer.Location))
                throw new ArgumentException("Location is required", nameof(customer));

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<bool> UpdateCustomerAsync(Guid id, Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (id != customer.Id)
                return false;

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CustomerExistsAsync(Guid id)
        {
            return await _context.Customers.AnyAsync(e => e.Id == id);
        }
    }
}
