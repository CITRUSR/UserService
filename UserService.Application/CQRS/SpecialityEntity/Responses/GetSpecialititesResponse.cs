namespace UserService.Application.CQRS.SpecialityEntity.Responses;

public record GetSpecialitiesResponse
{
    public int LastPage { get; set; }
    public List<SpecialityViewModel> Specialities { get; set; }
}
