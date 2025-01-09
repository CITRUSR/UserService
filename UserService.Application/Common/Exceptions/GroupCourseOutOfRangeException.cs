using Grpc.Core;
using Newtonsoft.Json;
using UserService.Application.Common.Errors;
using UserService.Domain.Entities;

namespace UserService.Application.Common.Exceptions;

public class GroupCourseOutOfRangeException : RpcException
{
    public GroupCourseOutOfRangeException(params Group[] groups)
        : base(
            new Status(
                StatusCode.OutOfRange,
                "The groups:"
                    + String.Join(", ", groups.Select(x => x.ToString()))
                    + " cannot be promoted to a higher course because it is already at the maximum course."
            )
        ) { }
}
