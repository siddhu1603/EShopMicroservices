﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.Infrastructure.Data.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.MigrateAsync().GetAwaiter().GetResult();

            await SeedAsync(context);
        }
        
        private static async Task SeedAsync(ApplicationDbContext context)
        {
            await SeedCustomerAsync(context);
            await SeedProductAsync(context);
            await SeedOrdersWithItemsAsync(context);
        }

        private static async Task SeedCustomerAsync(ApplicationDbContext context)
        {
            if(!await context.Customers.AnyAsync())
            {
                await context.AddRangeAsync(InitialData.Customers);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProductAsync(ApplicationDbContext context)
        {
            if (!await context.Products.AnyAsync())
            {
                await context.AddRangeAsync(InitialData.Products);
                await context.SaveChangesAsync();   //here the Auditable Interceptor is invoked.
            }
        }

        private static async Task SeedOrdersWithItemsAsync(ApplicationDbContext context)
        {
            if (!await context.Orders.AnyAsync())
            {
                await context.AddRangeAsync(InitialData.OrdersWithItems);
                await context.SaveChangesAsync();    //here the Auditable Interceptor is invoked.
            }
        }
    }
}
