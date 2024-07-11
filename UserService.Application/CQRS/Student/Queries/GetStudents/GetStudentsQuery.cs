using MediatR;

namespace UserService.Application.CQRS.Student.Queries.GetStudents;

public class GetStudentsQuery : IRequest<List<Domain.Entities.Student>>
{
    public string? SearchString { get; set; } = String.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public SortState SortState { get; set; } = SortState.LastNameAsc;
}