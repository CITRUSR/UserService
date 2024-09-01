namespace UserService.Application.CQRS.GroupEntity.Responses;

public record GroupDto
{
    public int Id { get; set; }
    public int SpecialityId { get; set; }
    public Guid CuratorId { get; set; }
    public byte CurrentSemster { get; set; }
    public byte CurrentCourse { get; set; }
    public byte SubGroup { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? GraduatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
