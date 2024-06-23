using Grpc.Core;

namespace UserService.Application.Common.Exceptions;

public class NotFoundException : RpcException
{
    public NotFoundException(string msg) : base(new Status(StatusCode.NotFound, msg))
    {
    }
}