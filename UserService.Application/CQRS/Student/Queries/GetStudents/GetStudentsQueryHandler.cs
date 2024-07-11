using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UserService.Application.CQRS.Student.Queries.GetStudents;

public class GetStudentsQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GetStudentsQuery, List<Domain.Entities.Student>>
{
    public async Task<List<Domain.Entities.Student>> Handle(GetStudentsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Domain.Entities.Student> students = DbContext.Students;

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

        var studentsRes = await students.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return studentsRes;
    }
}