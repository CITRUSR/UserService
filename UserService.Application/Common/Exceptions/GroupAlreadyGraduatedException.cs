using System.Text.Json;
using Grpc.Core;
using UserService.Application.Common.Errors;
using UserService.Domain.Entities;

namespace UserService.Application.Common.Exceptions;

public class GroupAlreadyGraduatedException : RpcException
{
    public GroupAlreadyGraduatedException(params Group[] groups)
        : base(
            new Status(
                StatusCode.Aborted,
                JsonSerializer.Serialize(
                    new Error
                    {
                        Erorrs =
                        [
                            "The groups:"
                                + String.Join(", ", groups.Select(x => x.ToString()))
                                + " cannot be graduated because it is already graduated."
                        ]
                    }
                )
            )
        ) { }
}
