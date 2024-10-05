using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace HelpMe.Identity.Data.IdentityServerConfig
{
    public class ConfigurationDbContextFactory : IDbContextFactory<ConfigurationDbContext>
    {
        public ConfigurationDbContext CreateDbContext()
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();


            string? assemblyName = typeof(ConfigurationDbContext).Assembly.GetName().Name;
            DbContextOptionsBuilder<ConfigurationDbContext> optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
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

            return new ConfigurationDbContext(optionsBuilder.Options);
        }

        //public ConfigurationDbContext CreateDbContext()
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}