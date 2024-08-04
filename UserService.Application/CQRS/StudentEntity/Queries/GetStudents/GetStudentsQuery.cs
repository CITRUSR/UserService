using MediatR;
using UserService.Application.Common.Paging;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudents;

public class GetStudentsQuery : IRequest<PaginationList<Student>>
{
    public string? SearchString { get; set; } = String.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public SortState SortState { get; set; } = SortState.LastNameAsc;
    public StudentDroppedOutStatus DroppedOutStatus { get; set; } = StudentDroppedOutStatus.All;
}
