using FluentValidation;
using MediatR;
using Ordering.Domain.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Ordering.API.Infrastructure.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest>[] _validators;

        public ValidatorBehavior(
            ILogger<ValidatorBehavior<TRequest, TResponse>> logger, 
            IValidator<TRequest>[] validators)
        {
            _logger = logger;
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation("Validating {RequestName} - validators: {Count}; value: {@RequestValue}", typeof(TRequest).Name, _validators.Length, JsonConvert.SerializeObject(request));

            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                _logger.LogWarning("{RequestName} validation failures:\n  {Failures}", typeof(TRequest).Name, string.Join("\n  ", failures.Select(err => err.ErrorMessage)));

                throw new OrderingDomainException(
                    $"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
            }

            return next();
        }
    }
}
