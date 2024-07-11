using Grpc.Core;

namespace UserService.Application.Common.Exceptions;

public class TeacherNotFoundException : RpcException
{
    public TeacherNotFoundException(Guid id) : base(new Status(Grpc.Core.StatusCode.NotFound,
        $"The teacher with id:{id} is not found"))
    {
    }
}