using MediatR;

namespace UserService.Application.CQRS.Student.Queries.GetStudentBySsoId;

public record GetStudentBySsoIdQuery(Guid SsoId) : IRequest<Domain.Entities.Student>;