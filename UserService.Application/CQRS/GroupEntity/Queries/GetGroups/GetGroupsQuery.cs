using MediatR;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Application.Enums;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroups;

public record GetGroupsQuery : IRequest<GetGroupsResponse>
{
    public string? SearchString { get; set; } = String.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public GroupSortState SortState { get; set; } = GroupSortState.GroupAsc;
    public GroupGraduatedStatus GraduatedStatus { get; set; } = GroupGraduatedStatus.OnlyActive;
    public DeletedStatus DeletedStatus { get; set; } = DeletedStatus.OnlyActive;
}
