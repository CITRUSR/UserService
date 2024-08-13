using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Paging;
using UserService.Application.Enums;
using UserService.Application.Extensions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroups;

public class GetGroupsQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<GetGroupsQuery, PaginationList<Group>>
{
    public async Task<PaginationList<Group>> Handle(
        GetGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Group> groups = DbContext.Groups;

        if (String.IsNullOrWhiteSpace(request.SearchString) == false)
        {
            groups = groups.Where(x =>
                (x.CurrentCourse + "-" + x.Speciality.Abbreavation + x.SubGroup).Contains(
                    request.SearchString
                )
            );
        }

        groups = groups.FilterByDeletedStatus<Group>(request.DeletedStatus, gr => gr.IsDeleted);

        groups = GetFilteredByGraduatedStatus(groups, request.GraduatedStatus);

        groups = GetSortedBySortState(groups, request.SortState);

        return await PaginationList<Group>.CreateAsync(groups, request.Page, request.PageSize);
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
                    .ThenBy(x => x.Speciality.Abbreavation)
                    .ThenBy(x => x.SubGroup),
            GroupSortState.GroupDesc
                => groups
                    .OrderByDescending(x => x.CurrentCourse)
                    .ThenBy(x => x.Speciality.Abbreavation)
                    .ThenBy(x => x.SubGroup),
        };

        return groups;
    }
}
