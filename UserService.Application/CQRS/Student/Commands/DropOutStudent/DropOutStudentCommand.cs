using MediatR;

namespace UserService.Application.CQRS.Student.Commands.DropOutStudent;

public record DropOutStudentCommand(long Id,DateTime DroppedOutTime) : IRequest<long>;