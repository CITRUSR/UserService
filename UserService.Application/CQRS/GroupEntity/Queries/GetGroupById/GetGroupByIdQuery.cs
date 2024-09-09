using MediatR;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;

public record GetGroupByIdQuery(int Id) : IRequest<GroupDto>;
