using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;

public record GetStudentBySsoIdQuery(Guid SsoId) : IRequest<Student>;