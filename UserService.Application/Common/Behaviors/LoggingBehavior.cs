using System.Diagnostics;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace UserService.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = request.GetType().Name;
        var requestGuid = Guid.NewGuid().ToString();

        var requestNameWithGuid = $"{requestName} [{requestGuid}]";

        Log.Information("[START] {RequestNameWithGuid}", requestNameWithGuid);
        TResponse response;

        var stopwatch = Stopwatch.StartNew();
        try
        {
            try
            {
                Log.Information(
                    "[PROPS] {RequestNameWithGuid} {Request}",
                    requestNameWithGuid,
                    JsonConvert.SerializeObject(request)
                );
            }
            catch (NotSupportedException)
            {
                Log.Information(
                    "[Serialization ERROR] {RequestNameWithGuid} Could not serialize the request.",
                    requestNameWithGuid
                );
            }

            response = await next();
        }
        finally
        {
            stopwatch.Stop();
            Log.Information(
                "[END] {RequestNameWithGuid}; ExecutionTimeInMs={ExecutionTimeMS}",
                requestNameWithGuid,
                stopwatch.ElapsedMilliseconds
            );
        }

        return response;
    }
}
