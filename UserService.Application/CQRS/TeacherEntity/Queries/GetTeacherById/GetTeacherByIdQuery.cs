using MediatR;
using UserService.Application.CQRS.TeacherEntity.Respones;

namespace UserService.Application.CQRS.TeacherEntity.Queries.GetTeacherById;

public record GetTeacherByIdQuery(Guid TeacherId) : IRequest<TeacherDto>;
