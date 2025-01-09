using Grpc.Core;
using Newtonsoft.Json;
using UserService.Application.Common.Errors;
using UserService.Domain.Entities;

namespace UserService.Application.Common.Exceptions;

public class StudentAlreadyDroppedOutException : RpcException
{
    public StudentAlreadyDroppedOutException(Student student)
        : base(
            new Status(
                StatusCode.Aborted,
                "The student:"
                    + $"{student.ToString()}"
                    + " cannot be dropped out because he is already dropped out."
            )
        ) { }

    public StudentAlreadyDroppedOutException(Student[] students)
        : base(
            new Status(
                StatusCode.Aborted,
                "The students:"
                    + $"{string.Join(", ", students.Select(x => x.ToString()))}"
                    + " cannot be dropped out because they are already dropped out."
            )
        ) { }
}
