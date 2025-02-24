﻿using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Order?> Get(int id)
        {
            return await context.Orders.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Add(Order order)
        {
            await context.Orders.AddAsync(order);
            return await context.SaveChangesAsync();
        }

        public async Task<int> Delete(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null) throw new Exception("Order does not exist");
            context.Orders.Remove(order);
            return await context.SaveChangesAsync();
        }

        public async Task<IList<Order>> GetAllOrdersByCustomer(int customerId)
        {
            return await context.Orders.Where(x => x.CustomerId == customerId).ToListAsync();
        }

        public async Task<IDictionary<int, double>> GetAverageArticlePerOrder()
        {
            var tmp = context.Orders.GroupBy(x => x, y => y, (x, y) => new
            {
                Order = x.Id,
                AverageArticle = y.Average(a => a.OrderDetails.Count)
            });
            return await tmp.ToDictionaryAsync(x => x.Order, y => y.AverageArticle);
        }

        public async Task<double> GetAverageOrderValue()
        {
            return await context.Orders.AverageAsync(x => x.TotalAmount);
        }
    }
}
