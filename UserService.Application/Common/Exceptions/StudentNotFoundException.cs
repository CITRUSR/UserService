using Grpc.Core;

namespace UserService.Application.Common.Exceptions;

public class StudentNotFoundException : RpcException
{
    public StudentNotFoundException(long id) : base(new Status(StatusCode.NotFound,
        $"The student with Id:{id} is not found"))
    {
    }
}