using FluentValidation;
using Grpc.Core;
using MediatR;
using Newtonsoft.Json;
using UserService.Application.Common.Errors;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UserService.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var validationContext = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(x => x.Validate(validationContext))
            .SelectMany(x => x.Errors)
            .Where(failure => failure != null)
            .ToList();

        if (failures.Any())
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    JsonConvert.SerializeObject(
                        new Error { Errors = failures.Select(x => x.ErrorMessage) }
                    )
                )
            );
        }

        return next();
    }
}
