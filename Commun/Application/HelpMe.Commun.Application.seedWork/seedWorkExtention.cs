using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Netcom.Commun.Application.seedWork.AutofacModules;
using Netcom.Commun.Application.seedWork.Filters;
using Netcom.Commun.Infra.Data.EF;
using Netcom.Commun.Infra.EventBus.Abstractions;
using System.Reflection;

namespace Netcom.Commun.Application.seedWork
{
    public static class seedWorkExtention
    {

        /// <summary>
        ///  register all IntegrationEvent  ( event bus IIntegrationEventHandler ) in assembly holding the Integration Event
        ///  Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
        ///  Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
        ///  Register the Command's Validators (Validators based on FluentValidation library)
        ///  register LoggingBehavior   ValidatorBehavior TransactionBehaviour
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="Configuration"></param>
        /// <returns></returns>
        public static ContainerBuilder AddNetcomApplicationSeedWork<TDDB>(this ContainerBuilder container, Assembly _CurrentAssembly, IConfiguration Configuration)
            where TDDB: DomaineBaseDbContext
        {
            container.RegisterType<TDDB>().As<DomaineBaseDbContext>().InstancePerLifetimeScope();
            container.RegisterAssemblyTypes(_CurrentAssembly)
             .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
            //typeof(T).GetTypeInfo().Assembly
            container.RegisterModule(new MediatorModule(_CurrentAssembly ));

            return container;
        }


        public static IServiceCollection AddNetcomMVCControllers(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                })
                   // Added for functional tests
                  // .AddApplicationPart(typeof(OrdersApiController).Assembly)
                   .AddNewtonsoftJson()
                   .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
               ;
            return services;
        }
    }
}
