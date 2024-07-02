using MediatR;

namespace UserService.Application.CQRS.Student.Commands.DeleteStudent;

public record DeleteStudentCommand(Guid Id) : IRequest<Guid>;