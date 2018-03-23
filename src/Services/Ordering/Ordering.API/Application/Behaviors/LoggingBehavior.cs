using System;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Infrastructure.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation($"Handling {typeof(TRequest).Name}");

            try
            {
                var response = await next();
                _logger.LogInformation($"Handled {typeof(TResponse).Name}");

                return response;
            }
            catch (Exception e)
            {
                Exception ex = e.InnerException ?? e;

                _logger.LogError(ex, "{ExceptionType} handling {RequestName}", ex.GetType().Name, typeof(TRequest).Name);

                return default(TResponse);
            }
        }
    }
}
