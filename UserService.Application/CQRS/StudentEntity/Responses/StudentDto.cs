namespace UserService.Application.CQRS.StudentEntity.Responses;

public record StudentDto
{
    public Guid Id { get; set; }
    public Guid SsoId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PatronymicName { get; set; }
    public int GroupId { get; set; }
    public DateTime? DroppedOutAt { get; set; }
    public bool IsDeleted { get; set; }
}
