using MediatR;

namespace UserService.Application.CQRS.Student.Commands.EditStudent;

public record EditStudentCommand(long Id, string FirstName, string LastName, string? PatronymicName, int GroupId)
    : IRequest<long>;