using MediatR;
using UserService.Application.Common.Paging;

namespace UserService.Application.CQRS.Group.Queries.GetGroups;

public class GetGroupsQueryHandler(IAppDbContext dbContext) : HandlerBase(dbContext),
    IRequestHandler<GetGroupsQuery, PaginationList<Domain.Entities.Group>>
{
    public async Task<PaginationList<Domain.Entities.Group>> Handle(GetGroupsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Domain.Entities.Group> groups = DbContext.Groups;

        if (String.IsNullOrWhiteSpace(request.SearchString))
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

        return await PaginationList<Domain.Entities.Group>.CreateAsync(groups, request.Page, request.PageSize);
    }
}