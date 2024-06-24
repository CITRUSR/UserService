using MediatR;

namespace UserService.Application.CQRS.Student.Commands.DeleteStudent;

public record DeleteStudentCommand(long Id) : IRequest<long>;