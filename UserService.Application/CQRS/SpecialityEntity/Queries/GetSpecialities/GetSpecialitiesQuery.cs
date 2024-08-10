using MediatR;
using UserService.Application.Common.Paging;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;

public class GetSpecialitiesQuery : IRequest<PaginationList<Speciality>>
{
    public string? SearchString { get; set; } = String.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public SpecialitySortState SortState { get; set; } = SpecialitySortState.NameAsc;
    public DeletedStatus DeletedStatus { get; set; } = DeletedStatus.OnlyActive;
}
