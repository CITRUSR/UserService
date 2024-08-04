﻿using System.Text.Json;
using Grpc.Core;
using UserService.Application.Common.Errors;
using UserService.Domain.Entities;

namespace UserService.Application.Common.Exceptions;

public class GroupCourseOutOfRangeException : RpcException
{
    public GroupCourseOutOfRangeException(params Group[] groups)
        : base(
            new Status(
                StatusCode.OutOfRange,
                JsonSerializer.Serialize(
                    new Error
                    {
                        Erorrs =
                        [
                            "The groups:"
                                + String.Join(", ", groups.Select(x => x.ToString()))
                                + " cannot be promoted to a higher course because it is already at the maximum course."
                        ]
                    }
                )
            )
        ) { }
}
