using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.Student.Commands.CreateStudent;

public record CreateStudentCommand(
    Guid SsoId,
    string FirstName,
    string LastName,
    string? PatronymicName,
    int GroupId) : IRequest<Guid>;