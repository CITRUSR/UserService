namespace UserService.Application.CQRS.TeacherEntity.Respones;

public class TeacherDto
{
    public Guid Id { get; set; }
    public Guid SsoId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PatronymicName { get; set; }
    public short RoomId { get; set; }
    public DateTime? FiredAt { get; set; }
    public bool IsDeleted { get; set; }
}
