using MediatR;

namespace UserService.Application.CQRS.Group.Queries.GetGroupById;

public record GetGroupByIdQuery(int Id) : IRequest<Domain.Entities.Group>;