using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.RecoveryStudents;

public record RecoveryStudentsCommand(List<Guid> StudentIds) : IRequest<List<StudentShortInfoDto>>;
