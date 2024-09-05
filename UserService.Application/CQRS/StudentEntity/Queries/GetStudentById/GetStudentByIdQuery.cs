using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;

public record GetStudentByIdQuery(Guid Id) : IRequest<StudentDto>;
