using BlazingPizza.Orders;
using Castle.Core.Configuration;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace unit
{
    public class orderstests
    {
        [Fact]
        public async Task TestSaveCallsDbService()
        {
            var db = new Moq.Mock<IOrdersService>();
            var ordersController = new OrdersController(db.Object);

            var order = new Order() { OrderId = Guid.NewGuid() };
            await ordersController.PlaceOrder(order);

            db.Verify(foo => foo.SaveOrder(order));
        }
    }
}
