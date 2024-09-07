namespace UserService.Application.CQRS.SpecialityEntity.Responses;

public record SpecialityViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
}
