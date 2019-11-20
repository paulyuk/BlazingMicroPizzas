using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazingPizza.Orders
{
    public class OrdersService : IOrdersService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<Order> _orders;

        public OrdersService(IConfiguration configuration)
        {
            _configuration = configuration;
            var client = new MongoClient(_configuration["Data:Connection"]);
            var database = client.GetDatabase(_configuration["Data:Database"]);
            _orders = database.GetCollection<Order>(_configuration["Data:Collection"]);
        }

        public async Task<IEnumerable<Order>> GetOrdersForUser(string userId)
        {
            return await _orders.Find(o => o.UserId == userId).ToListAsync();
        }

        public async Task<Order> GetOrder(Guid orderId)
        {
            return await _orders.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();
        }

        public async Task SaveOrder(Order order)
        {
            await _orders.InsertOneAsync(order);
        }
    }

    public interface IOrdersService
    {
        Task<IEnumerable<Order>> GetOrdersForUser(string userId);
        Task<Order> GetOrder(Guid orderId);
        Task SaveOrder(Order order);
    }
}
