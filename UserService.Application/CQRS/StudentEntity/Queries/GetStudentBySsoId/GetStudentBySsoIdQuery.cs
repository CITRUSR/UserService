using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;

public record GetStudentBySsoIdQuery(Guid SsoId) : IRequest<StudentDto>;
