using MediatR;
using UserService.Application.Common.Paging;

namespace UserService.Application.CQRS.Student.Queries.GetStudents;

public class GetStudentsQuery : IRequest<PaginationList<Domain.Entities.Student>>
{
    public string? SearchString { get; set; } = String.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public SortState SortState { get; set; } = SortState.LastNameAsc;
}