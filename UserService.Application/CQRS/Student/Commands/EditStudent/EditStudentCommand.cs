using MediatR;

namespace UserService.Application.CQRS.Student.Commands.EditStudent;

public record EditStudentCommand(Guid Id, string FirstName, string LastName, string? PatronymicName, int GroupId)
    : IRequest<Guid>;