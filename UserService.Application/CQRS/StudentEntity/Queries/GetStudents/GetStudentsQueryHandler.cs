using MediatR;
using UserService.Application.Common.Paging;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudents;

public class GetStudentsQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GetStudentsQuery, PaginationList<Student>>
{
    public async Task<PaginationList<Student>> Handle(GetStudentsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Student> students = DbContext.Students;

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            students = students.Where(x =>
                x.FirstName.Contains(request.SearchString) || x.LastName.Contains(request.SearchString) ||
                x.PatronymicName.Contains(request.SearchString) || x.Group.ToString().Contains(request.SearchString));
        }

        students = request.SortState switch
        {
            SortState.FistNameAsc => students.OrderBy(s => s.FirstName),
            SortState.FirstNameDesc => students.OrderByDescending(s => s.FirstName),
            SortState.LastNameAsc => students.OrderBy(s => s.LastName),
            SortState.LastNameDesc => students.OrderByDescending(s => s.LastName),
            SortState.GroupAsc => students.OrderBy(s => s.Group.CurrentSemester)
                .ThenBy(s => s.Group.Speciality.Abbreavation).ThenBy(s => s.Group.SubGroup),
            SortState.GroupDesc => students.OrderByDescending(s => s.Group.CurrentSemester)
        };

        return await PaginationList<Student>.CreateAsync(students, request.Page, request.PageSize);
    }
}