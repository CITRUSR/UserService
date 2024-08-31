using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Commands.EditStudent;

public record EditStudentCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? PatronymicName,
    int GroupId,
    bool IsDeleted
) : IRequest<Student>;
