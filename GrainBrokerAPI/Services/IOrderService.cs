using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainBrokerAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(Guid id);
        Task<Order> CreateOrderAsync(Order order);
        Task<bool> UpdateOrderAsync(Guid id, Order order);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<bool> OrderExistsAsync(Guid id);
    }
}
