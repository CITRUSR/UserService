using Grpc.Core;
using Newtonsoft.Json;
using UserService.Application.Common.Errors;
using UserService.Domain.Entities;

namespace UserService.Application.Common.Exceptions;

public class GroupAlreadyGraduatedException : RpcException
{
    public GroupAlreadyGraduatedException(params Group[] groups)
        : base(
            new Status(
                StatusCode.Aborted,
                "The groups:"
                    + String.Join(", ", groups.Select(x => x.ToString()))
                    + " cannot be graduated because it is already graduated."
            )
        ) { }
}
