using MediatR;

namespace UserService.Application.CQRS.Student.Commands.DropOutStudent;

public record DropOutStudentCommand(Guid Id,DateTime DroppedOutTime) : IRequest<Guid>;