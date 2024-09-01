namespace UserService.Application.CQRS.GroupEntity.Responses;

public record GroupShortInfoDto
{
    public int Id { get; set; }
    public string GroupName { get; set; }
}
