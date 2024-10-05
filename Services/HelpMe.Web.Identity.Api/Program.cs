
using Duende.IdentityServer.EntityFramework.DbContexts;
using HelpMe.Commun.Infra.Logging.Serilog;
using HelpMe.Identity.Data.IdentityServerConfig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace HelpMe.Web.Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();



            var configuration = builder.Build();

            host.MigrateDbContext<PersistedGrantDbContext>((_, __) => { })

                .MigrateDbContext<ConfigurationDbContext>((context, services) =>
                {
                    new ConfigurationDbContextSeed()
                        .SeedAsync(context, configuration)
                        .Wait();
                });


            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseStartup<Startup>().UseHelpMeSerilog();
                    
                });
    }
}
