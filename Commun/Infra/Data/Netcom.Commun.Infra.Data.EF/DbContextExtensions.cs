using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace HelpMe.Commun.Infra.Data.EF
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddNetcomDomaineDbContext<TDB>(this IServiceCollection services, IConfiguration configuration)
            where TDB : DomaineBaseDbContext
        {
            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<TDB>(options =>
                   {
                       options.UseSqlServer(configuration["ConnectionString"],
                           sqlServerOptionsAction: sqlOptions =>
                           {
                               sqlOptions.MigrationsAssembly(typeof(TDB).GetTypeInfo().Assembly.GetName().Name);
                              // sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                           });
                   },

                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );

            services.AddScoped<DomaineBaseDbContext, TDB>();

            return services;
        }
    }
}
