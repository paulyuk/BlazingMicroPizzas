﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Orders
{
    [Route("orders")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly IOrdersService _db;

        public OrdersController(IOrdersService db)
        {
            _db = db;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<OrderWithStatus>>> GetOrders(string userId)
        {
            var orders = await _db.GetOrdersForUser(userId);

            return orders.Select(o => OrderWithStatus.FromOrder(o)).ToList();
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(Guid orderId)
        {
            Order order = await _db.GetOrder(orderId);

            if (order == null)
            {
                return NotFound();
            }

            return OrderWithStatus.FromOrder(order);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> PlaceOrder(Order order)
        {
            order.CreatedTime = DateTime.UtcNow;
            order.DeliveryLocation = new LatLong(51.5001, -0.1239);
            //TODO: Should we let MongoDB do this?
            order.OrderId = Guid.NewGuid();
            order.TotalPrice = order.GetFormattedTotalPrice();
            await _db.SaveOrder(order);
            OrdersEventSource.Log.OrderCreated();
            return order.OrderId;
        }
    }
}
