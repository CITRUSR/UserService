namespace UserService.Application.CQRS.StudentEntity.Responses;

public record StudentShortInfoDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
