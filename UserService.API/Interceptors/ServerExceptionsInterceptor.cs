using Grpc.Core;
using Grpc.Core.Interceptors;
using Serilog;

namespace UserService.API.Interceptors;

public class ServerExceptionsInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception e)
        {
            switch (e)
            {
                case RpcException rpc:
                    Log.Error(rpc.Status.Detail);
                    break;
                default:
                    Log.Error(e.Message);
                    break;
            }

            throw;
        }
    }
}
