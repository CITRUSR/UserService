namespace UserService.Application.CQRS.GroupEntity.Responses;

public record GroupViewModel
{
    public int Id { get; set; }
    public int StudentCount { get; set; }
    public string CuratorFirstName { get; set; }
    public string CuratorLastName { get; set; }
    public string? CuratorPatronymicName { get; set; }
    public string GroupName { get; set; }
}
