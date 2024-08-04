using Grpc.Core;

namespace UserService.Application.Common.Exceptions;

public class GroupNotFoundException : RpcException
{
    public GroupNotFoundException(int id)
        : base(new Status(Grpc.Core.StatusCode.NotFound, $"The group with id:{id} is not found"))
    { }

    public GroupNotFoundException(int[] id)
        : base(
            new Status(
                Grpc.Core.StatusCode.NotFound,
                $"The groups with id:{string.Join(", ", id)} are not found"
            )
        ) { }
}
