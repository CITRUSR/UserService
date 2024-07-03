using MediatR;

namespace UserService.Application.CQRS.Student.Quereis;

public record GetStudentByIdQuery(Guid Id) : IRequest<Domain.Entities.Student>;