using MediatR;
using UserService.Application.CQRS.TeacherEntity.Respones;

namespace UserService.Application.CQRS.TeacherEntity.Commands.CreateTeacher;

public record CreateTeacherCommand(
    Guid SsoId,
    string FirstName,
    string LastName,
    string? PatronymicName,
    short RoomId
) : IRequest<TeacherShortInfoDto>;
