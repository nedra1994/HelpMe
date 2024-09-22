using System.Linq;
using System.Reflection;
using Autofac;
using FluentValidation;
using MediatR;
using Netcom.Commun.Application.seedWork.Behaviors;
using Netcom.Commun.Infra.EventBus.Abstractions;


namespace Netcom.Commun.Application.seedWork.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        Assembly CurrentAssembly;
        public MediatorModule(Assembly _CurrentAssembly)
        {
            CurrentAssembly = _CurrentAssembly;
        }
        //typeof(CreateOrderCommandHandler).GetTypeInfo().Assembly
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(CurrentAssembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
            builder.RegisterAssemblyTypes(CurrentAssembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register the Command's Validators (Validators based on FluentValidation library)
            builder
                .RegisterAssemblyTypes(CurrentAssembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(CurrentAssembly)
              .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

            //builder.Register<ServiceFactory>(context =>
            //{
            //    var componentContext = context.Resolve<IComponentContext>();
            //    return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            //});

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));

        }
    }
}
