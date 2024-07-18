using MediatR;

namespace UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;

public record CreateStudentCommand(
    Guid SsoId,
    string FirstName,
    string LastName,
    string? PatronymicName,
    int GroupId) : IRequest<Guid>;