namespace UserService.Application.CQRS.SpecialityEntity.Responses;

public record SpecialityDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public decimal Cost { get; set; }
    public byte DurationMonths { get; set; }
    public bool IsDeleted { get; set; }
}
