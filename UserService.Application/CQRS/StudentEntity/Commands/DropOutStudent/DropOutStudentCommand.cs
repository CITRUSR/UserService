using MediatR;

namespace UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;

public record DropOutStudentCommand(Guid Id, DateTime DroppedOutTime) : IRequest<Guid>;
