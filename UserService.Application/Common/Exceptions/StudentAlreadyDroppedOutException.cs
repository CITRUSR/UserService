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
                JsonConvert.SerializeObject(
                    new Error
                    {
                        Erorrs =
                        [
                            "The student:"
                                + $"{student.ToString()}"
                                + " cannot be dropped out because he is already dropped out."
                        ]
                    }
                )
            )
        ) { }

    public StudentAlreadyDroppedOutException(Student[] students)
        : base(
            new Status(
                StatusCode.Aborted,
                JsonConvert.SerializeObject(
                    new Error
                    {
                        Erorrs =
                        [
                            "The students:"
                                + $"{string.Join(", ", students.Select(x => x.ToString()))}"
                                + " cannot be dropped out because they are already dropped out."
                        ]
                    }
                )
            )
        ) { }
}
