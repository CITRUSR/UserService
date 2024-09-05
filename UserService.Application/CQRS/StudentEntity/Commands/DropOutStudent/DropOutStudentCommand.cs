using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;

public record DropOutStudentCommand(Guid Id, DateTime DroppedOutTime)
    : IRequest<StudentShortInfoDto>;
