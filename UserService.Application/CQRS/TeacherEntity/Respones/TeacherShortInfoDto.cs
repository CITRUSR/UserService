namespace UserService.Application.CQRS.TeacherEntity.Respones;

public record TeacherShortInfoDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
