using System.Text.Json;
using Grpc.Core;
using UserService.Application.Common.Errors;

namespace UserService.Application.Common.Exceptions;

public class GroupCourseOutOfRangeException : RpcException
{
    public GroupCourseOutOfRangeException(int[] id) : base(new Status(StatusCode.OutOfRange,
        JsonSerializer.Serialize(new Error
        {
            Erorrs =
            [
                "The groups with id:" + String.Join(", ", id) +
                " cannot be promoted to a higher course because it is already at the maximum course."
            ]
        })))
    {
    }
}