namespace UserService.Application.CQRS.GroupEntity.Responses;

public record GetGroupsResponse
{
    public int LastPage { get; set; }
    public List<GroupViewModel> Groups { get; set; }
}
