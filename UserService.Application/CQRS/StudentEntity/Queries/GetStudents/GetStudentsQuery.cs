using MediatR;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Application.Enums;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudents;

public record GetStudentsQuery : IRequest<GetStudentsResponse>
{
    public string? SearchString { get; set; } = String.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public SortState SortState { get; set; } = SortState.LastNameAsc;
    public StudentDroppedOutStatus DroppedOutStatus { get; set; } =
        StudentDroppedOutStatus.OnlyActive;
    public DeletedStatus DeletedStatus { get; set; } = DeletedStatus.OnlyActive;
}
