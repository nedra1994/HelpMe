
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using HelpMe.Commun.Infra.Logging.Abstraction;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HelpMe.Commun.Infra.Logging.Serilog
{
    public static class HelpMeSerilogExtention
    {
        public static IWebHostBuilder UseHelpMeSerilog(this IWebHostBuilder webBuilder)
        {

            return webBuilder.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
        }
        //public static IHostBuilder UseHelpMeSerilog(this IHostBuilder hostBuilder)
        //{
        //    return hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
        //        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
        //}

        public static IServiceCollection AddHelpMeSerilog<T>(this IServiceCollection services)
            where T : class, IHttpContextAccessor
        {
            services.AddSingleton(typeof(IHelpMeLog<>), typeof(SeriHelpMeLog<>));
            services.TryAddSingleton<IHttpContextAccessor, T>();
            return services;
        }
    }
}
