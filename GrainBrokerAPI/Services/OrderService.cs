using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GrainBrokerAPI.Services
{
    /// <summary>
    /// Service implementation for Order business logic
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly GrainBrokerContext _context;

        public OrderService(GrainBrokerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.GrainOrders.ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _context.GrainOrders.FindAsync(id);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Business validation
            if (order.OrderReqAmtTon <= 0)
                throw new ArgumentException("Order request amount must be greater than zero", nameof(order));

            if (order.SuppliedAmtTon < 0)
                throw new ArgumentException("Supplied amount cannot be negative", nameof(order));

            if (order.CostOfDelivery < 0)
                throw new ArgumentException("Cost of delivery cannot be negative", nameof(order));

            _context.GrainOrders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<bool> UpdateOrderAsync(Guid id, Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (id != order.Id)
                return false;

            // Business validation
            if (order.OrderReqAmtTon <= 0)
                throw new ArgumentException("Order request amount must be greater than zero", nameof(order));

            if (order.SuppliedAmtTon < 0)
                throw new ArgumentException("Supplied amount cannot be negative", nameof(order));

            if (order.CostOfDelivery < 0)
                throw new ArgumentException("Cost of delivery cannot be negative", nameof(order));

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _context.GrainOrders.FindAsync(id);

            if (order == null)
                return false;

            _context.GrainOrders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> OrderExistsAsync(Guid id)
        {
            return await _context.GrainOrders.AnyAsync(e => e.Id == id);
        }
    }
}
