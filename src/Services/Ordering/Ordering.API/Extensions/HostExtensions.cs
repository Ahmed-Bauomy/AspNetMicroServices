using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host,Action<TContext,IServiceProvider> seeder,int retry = 0) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Begin Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                    context.Database.Migrate();
                    seeder(context, services);

                    logger.LogInformation("End of Migrating database associated with context {DbContextName}",typeof(TContext).Name);
                }
                catch (SqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}",typeof(TContext).Name);
                    if(retry < 50)
                    {
                        retry++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host,seeder, retry);
                    }
                }

                
            }
            return host;
        }
    }
}
