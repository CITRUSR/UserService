using Grpc.Core;

namespace UserService.Application.Common.Exceptions;

public class SpecialityNotFoundException : RpcException
{
    public SpecialityNotFoundException(params int[] ids)
        : base(
            new Status(
                StatusCode.NotFound,
                $"The specialities with id:{string.Join(", ", ids)} are not found"
            )
        ) { }
}
