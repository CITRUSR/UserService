using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;

public record GetStudentByIdQuery(Guid Id) : IRequest<Student>;