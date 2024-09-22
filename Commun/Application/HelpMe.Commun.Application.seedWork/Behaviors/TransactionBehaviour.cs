using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netcom.Commun.Infra.Data.EF;
using Netcom.Commun.Infra.EventBus.Extensions;
using Netcom.Commun.Infra.IntegrationEventLogEF;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netcom.Commun.Application.seedWork.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly DomaineBaseDbContext _dbContext;
       // private readonly IEventBusIntegrationEventService _IntegrationEventService;

        public TransactionBehaviour(DomaineBaseDbContext dbContext,
           // IEventBusIntegrationEventService integrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(DomaineBaseDbContext));
           // _IntegrationEventService = integrationEventService ?? throw new ArgumentException(nameof(integrationEventService));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {

                    //string aa = "dddddd";
                    //aa.tob
                    Guid transactionId;

                    using (var transaction = await _dbContext.BeginTransactionAsync())
                   // using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        await _dbContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                   // await _IntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);


                throw ex;
            }
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {

                    //string aa = "dddddd";
                    //aa.tob
                    Guid transactionId;

                    using (var transaction = await _dbContext.BeginTransactionAsync())
                    // using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        await _dbContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                    // await _IntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);


                throw ex;
            }
        }
    }
}
