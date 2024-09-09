namespace UserService.Application.CQRS.StudentEntity.Responses;

public record GetStudentsResponse
{
    public int LastPage { get; set; }
    public List<StudentViewModel> Students { get; set; }
}
