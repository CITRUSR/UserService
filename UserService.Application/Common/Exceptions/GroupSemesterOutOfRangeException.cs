using Grpc.Core;
using Newtonsoft.Json;
using UserService.Application.Common.Errors;
using UserService.Domain.Entities;

namespace UserService.Application.Common.Exceptions;

public class GroupSemesterOutOfRangeException : RpcException
{
    public GroupSemesterOutOfRangeException(params Group[] groups)
        : base(
            new Status(
                StatusCode.OutOfRange,
                JsonConvert.SerializeObject(
                    new Error
                    {
                        Erorrs =
                        [
                            "The groups:"
                                + String.Join(", ", groups.Select(x => x.ToString()))
                                + " cannot be promoted to a higher semester because it is already at the maximum semester."
                        ]
                    }
                )
            )
        ) { }
}
