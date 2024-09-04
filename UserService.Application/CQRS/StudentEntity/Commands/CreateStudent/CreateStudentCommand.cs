using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;

public record CreateStudentCommand(
    Guid SsoId,
    string FirstName,
    string LastName,
    string? PatronymicName,
    int GroupId
) : IRequest<StudentShortInfoDto>;
