using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;

public record DeleteStudentsCommand(List<Guid> StudentIds) : IRequest<List<StudentShortInfoDto>>;
