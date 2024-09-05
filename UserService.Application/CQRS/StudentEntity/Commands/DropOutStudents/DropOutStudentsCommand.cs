using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;

public record DropOutStudentsCommand(List<Guid> StudentIds, DateTime DroppedOutTime)
    : IRequest<List<StudentShortInfoDto>>;
