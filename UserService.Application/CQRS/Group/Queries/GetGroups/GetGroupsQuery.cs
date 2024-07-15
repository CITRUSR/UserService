using MediatR;
using UserService.Application.Common.Paging;

namespace UserService.Application.CQRS.Group.Queries.GetGroups;

public class GetGroupsQuery : IRequest<PaginationList<Domain.Entities.Group>>
{
    public string? SearchString { get; set; } = String.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public GroupSortState? SortState { get; set; } = GroupSortState.GroupAsc;
}