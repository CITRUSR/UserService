using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.SoftDeleteStudents;

public record SoftDeleteStudentsCommand(List<Guid> StudentIds)
    : IRequest<List<StudentShortInfoDto>>;
