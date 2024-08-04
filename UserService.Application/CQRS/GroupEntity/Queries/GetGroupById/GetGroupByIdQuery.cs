using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;

public record GetGroupByIdQuery(int Id) : IRequest<Group>;
