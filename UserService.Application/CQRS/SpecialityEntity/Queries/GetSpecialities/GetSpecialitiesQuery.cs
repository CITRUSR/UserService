using MediatR;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Application.Enums;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;

public record GetSpecialitiesQuery : IRequest<GetSpecialitiesResponse>
{
    public string? SearchString { get; set; } = String.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public SpecialitySortState SortState { get; set; } = SpecialitySortState.NameAsc;
    public DeletedStatus DeletedStatus { get; set; } = DeletedStatus.OnlyActive;
}
