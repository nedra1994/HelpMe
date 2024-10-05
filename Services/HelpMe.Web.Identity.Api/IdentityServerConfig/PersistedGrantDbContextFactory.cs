using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace HelpMe
    .Identity.Data.IdentityServerConfig
{
    public class PersistedGrantDbContextFactory : IDbContextFactory<PersistedGrantDbContext>
    {
       

        public PersistedGrantDbContext CreateDbContext()
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();


            string? assemblyName = typeof(PersistedGrantDbContext).Assembly.GetName().Name;
            DbContextOptionsBuilder<PersistedGrantDbContext> optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            optionsBuilder.UseSqlServer(config["ConnectionString"], sqlOptions => {
                sqlOptions.MigrationsAssembly(assemblyName);
            });


            IServiceCollection services = new ServiceCollection();

            services.AddIdentityServer()
                    .AddOperationalStore(options => {
                        options.ConfigureDbContext = b => b.UseSqlServer(config["ConnectionString"], sqlOptions => {
                            sqlOptions.MigrationsAssembly(assemblyName);
                        });
                    });

            optionsBuilder.UseApplicationServiceProvider(services.BuildServiceProvider());

            return new PersistedGrantDbContext(optionsBuilder.Options);
        }
    }
}