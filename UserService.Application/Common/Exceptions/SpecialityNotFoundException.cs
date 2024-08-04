using Grpc.Core;

namespace UserService.Application.Common.Exceptions;

public class SpecialityNotFoundException : RpcException
{
    public SpecialityNotFoundException(int id)
        : base(new Status(StatusCode.NotFound, $"The speciality with id:{id} is not found")) { }
}
