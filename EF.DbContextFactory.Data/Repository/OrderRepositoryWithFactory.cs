﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EF.DbContextFactory.Examples.Data.Entity;
using EF.DbContextFactory.Examples.Data.Persistence;

namespace EF.DbContextFactory.Examples.Data.Repository
{
    public class OrderRepositoryWithFactory : IOrderRepository
    {
        private readonly Func<OrderContext> _factory;

        public OrderRepositoryWithFactory(Func<OrderContext> factory)
        {
            _factory = factory;
        }

        public void Add(Order order)
        {
            using (var context = _factory.Invoke())
            {
                context.Orders.Add(order);
                context.SaveChanges();
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            using (var context = _factory.Invoke())
            {
                return context.Orders.Include(x => x.OrderItems).ToList();
            }
        }

        public Order GetOrderById(Guid id)
        {
            using (var context = _factory())
            {
                return context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);
            }
        }

        public void DeleteById(Guid id)
        {
            using (var context = _factory.Invoke())
            {
                var order = context.Orders.FirstOrDefault(x => x.Id == id);
                context.Entry(order).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public async Task<int> Update(Order order)
        {
            using (var context = _factory.Invoke())
            {
                context.Orders.Attach(order);
                context.Entry(order).State = EntityState.Modified;
                return await context.SaveChangesAsync();
            }
        }
    }
}