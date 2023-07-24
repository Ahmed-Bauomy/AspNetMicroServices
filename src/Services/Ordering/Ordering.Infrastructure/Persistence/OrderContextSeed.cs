using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public async static void SeedDataAsync(OrderContext dbContext,ILogger<OrderContextSeed> logger)
        {
            if (!dbContext.Orders.Any())
            {
                dbContext.Orders.AddRange(GetPredefinedOrders());
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Data succssfully seeded with context {dbContextName}",typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPredefinedOrders()
        {
            return new List<Order>()
            {
                new Order() 
                {
                    UserName = "swn", 
                    FirstName = "Mehmet", 
                    LastName = "Ozkaya", 
                    EmailAddress = "ezozkme@gmail.com", 
                    AddressLine = "Bahcelievler", 
                    Country = "Turkey", 
                    TotalPrice = 350 
                }
            };
        }
    }
}
