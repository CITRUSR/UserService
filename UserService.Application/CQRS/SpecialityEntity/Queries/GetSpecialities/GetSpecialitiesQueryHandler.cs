using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Paging;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;

public class GetSpecialitiesQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GetSpecialitiesQuery, PaginationList<Speciality>>
{
    public async Task<PaginationList<Speciality>> Handle(GetSpecialitiesQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Speciality> specialities = DbContext.Specialities;

        specialities = GetFilteredByDeletedStatus(specialities, request.DeletedStatus);

        if (String.IsNullOrWhiteSpace(request.SearchString) == false)
        {
            specialities = specialities.Where(x => x.Name.Contains(request.SearchString) ||
                                                   x.Abbreavation.Contains(request.SearchString));
        }

        specialities = GetSortedBySortState(specialities, request.SortState);

        return await PaginationList<Speciality>.CreateAsync(specialities, request.Page, request.PageSize);
    }

    private IQueryable<Speciality> GetFilteredByDeletedStatus(IQueryable<Speciality> specialities,
        SpecialityDeletedStatus deletedStatus)
    {
        specialities = deletedStatus switch
        {
            SpecialityDeletedStatus.All => specialities,
            SpecialityDeletedStatus.OnlyDeleted => specialities.Where(x => x.IsDeleted == true),
            SpecialityDeletedStatus.OnlyActive => specialities.Where(x => x.IsDeleted == false),
        };

        return specialities;
    }

    private IQueryable<Speciality> GetSortedBySortState(IQueryable<Speciality> specialities,
        SpecialitySortState sortState)
    {
        specialities = sortState switch
        {
            SpecialitySortState.NameAsc => specialities.OrderBy(x => x.Name),
            SpecialitySortState.NameDesc => specialities.OrderByDescending(x => x.Name),
            SpecialitySortState.AbbreviationAsc => specialities.OrderBy(x => x.Abbreavation),
            SpecialitySortState.AbbreviationDesc => specialities.OrderByDescending(x => x.Abbreavation),
            SpecialitySortState.CostAsc => specialities.OrderBy(x => x.Cost),
            SpecialitySortState.CostDesc => specialities.OrderByDescending(x => x.Cost),
            SpecialitySortState.DurationMonthsAsc => specialities.OrderBy(x => x.DurationMonths),
            SpecialitySortState.DurationMonthsDesc => specialities.OrderByDescending(x => x.DurationMonths),
        };

        return specialities;
    }
}