using System.Text.Json;
using Grpc.Core;
using UserService.Application.Common.Errors;

namespace UserService.Application.Common.Exceptions;

public class GroupSemesterOutOfRangeException : RpcException
{
    public GroupSemesterOutOfRangeException(int[] groupsId) : base(new Status(StatusCode.OutOfRange,
        JsonSerializer.Serialize(new Error
        {
            Erorrs =
            [
                "The groups with id:" + String.Join(", ", groupsId) +
                " cannot be promoted to a higher semester because it is already at the maximum semester."
            ]
        })))
    {
    }
}