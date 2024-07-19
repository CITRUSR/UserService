using MediatR;
using UserService.Application.Common.Paging;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroups;

public class GetGroupsQueryHandler(IAppDbContext dbContext) : HandlerBase(dbContext),
    IRequestHandler<GetGroupsQuery, PaginationList<Group>>
{
    public async Task<PaginationList<Group>> Handle(GetGroupsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Group> groups = DbContext.Groups;

        if (String.IsNullOrWhiteSpace(request.SearchString) == false)
        {
            groups = groups.Where(x => x.Speciality.Abbreavation.Contains(request.SearchString) ||
                                       x.CurrentCourse.ToString().Contains(request.SearchString) ||
                                       x.SubGroup.ToString().Contains(request.SearchString));
        }

        groups = request.SortState switch
        {
            GroupSortState.GroupAsc => groups.OrderBy(x => x.CurrentCourse)
                .ThenBy(x => x.Speciality.Abbreavation)
                .ThenBy(x => x.SubGroup),
            GroupSortState.GroupDesc => groups.OrderByDescending(x => x.CurrentCourse)
                .ThenBy(x => x.Speciality.Abbreavation)
                .ThenBy(x => x.SubGroup),
        };

        return await PaginationList<Group>.CreateAsync(groups, request.Page, request.PageSize);
    }
}