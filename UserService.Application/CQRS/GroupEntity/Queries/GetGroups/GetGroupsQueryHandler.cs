using Mapster;
using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Application.Extensions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroups;

public class GetGroupsQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<GetGroupsQuery, GetGroupsResponse>
{
    public async Task<GetGroupsResponse> Handle(
        GetGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Group> groups = DbContext.Groups;

        if (String.IsNullOrWhiteSpace(request.SearchString) == false)
        {
            groups = groups.Where(x =>
                (x.CurrentCourse + "-" + x.Speciality.Abbreviation + x.SubGroup).Contains(
                    request.SearchString,
                    StringComparison.CurrentCultureIgnoreCase
                )
            );
        }

        groups = groups.FilterByDeletedStatus<Group>(request.DeletedStatus, gr => gr.IsDeleted);

        groups = GetFilteredByGraduatedStatus(groups, request.GraduatedStatus);

        groups = GetSortedBySortState(groups, request.SortState);

        var pagList = await PaginationList<Group>.CreateAsync(
            groups,
            request.Page,
            request.PageSize
        );

        return pagList.Adapt<GetGroupsResponse>();
    }

    private IQueryable<Group> GetFilteredByGraduatedStatus(
        IQueryable<Group> groups,
        GroupGraduatedStatus graduatedStatus
    )
    {
        groups = graduatedStatus switch
        {
            GroupGraduatedStatus.All => groups,
            GroupGraduatedStatus.OnlyActive => groups.Where(x => x.GraduatedAt == null),
            GroupGraduatedStatus.OnlyGraduated => groups.Where(x => x.GraduatedAt != null),
        };

        return groups;
    }

    private IQueryable<Group> GetSortedBySortState(
        IQueryable<Group> groups,
        GroupSortState sortState
    )
    {
        groups = sortState switch
        {
            GroupSortState.GroupAsc
                => groups
                    .OrderBy(x => x.CurrentCourse)
                    .ThenBy(x => x.Speciality.Abbreviation)
                    .ThenBy(x => x.SubGroup),
            GroupSortState.GroupDesc
                => groups
                    .OrderByDescending(x => x.CurrentCourse)
                    .ThenBy(x => x.Speciality.Abbreviation)
                    .ThenBy(x => x.SubGroup),
        };

        return groups;
    }
}
