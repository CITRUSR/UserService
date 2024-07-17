using MediatR;

namespace UserService.Application.CQRS.Teacher.Commands.CreateTeacher;

public record CreateTeacherCommand(
    Guid SsoId,
    string FirstName,
    string LastName,
    string? PatronymicName,
    short RoomId
) : IRequest<Guid>;