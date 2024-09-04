using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;

public record DeleteStudentCommand(Guid Id) : IRequest<StudentShortInfoDto>;
