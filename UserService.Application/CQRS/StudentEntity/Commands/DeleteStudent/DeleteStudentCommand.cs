using MediatR;

namespace UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;

public record DeleteStudentCommand(Guid Id) : IRequest<Guid>;
