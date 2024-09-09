using MediatR;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;

public record DeleteGroupsCommand(List<int> Ids) : IRequest<List<GroupShortInfoDto>>;
