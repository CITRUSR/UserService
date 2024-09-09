namespace UserService.Application.CQRS.StudentEntity.Responses;

public record StudentViewModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PatronymicName { get; set; }
    public string GroupName { get; set; }
}
