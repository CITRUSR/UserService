using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Paging;
using UserService.Application.Enums;
using UserService.Application.Extensions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudents;

public class GetStudentsQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<GetStudentsQuery, PaginationList<Student>>
{
    public async Task<PaginationList<Student>> Handle(
        GetStudentsQuery request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Student> students = DbContext.Students;

        students = students.FilterByDeletedStatus<Student>(
            request.DeletedStatus,
            st => st.IsDeleted
        );

        students = GetFilteredByDroppedOutStatus(students, request.DroppedOutStatus);

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            students = students.Where(x =>
                x.FirstName.Contains(request.SearchString)
                || x.LastName.Contains(request.SearchString)
                || x.PatronymicName.Contains(request.SearchString)
                || (
                    x.Group.CurrentCourse + "-" + x.Group.Speciality.Abbreavation + x.Group.SubGroup
                ).Contains(request.SearchString)
            );
        }

        students = GetSortedBySortState(students, request.SortState);

        return await PaginationList<Student>.CreateAsync(students, request.Page, request.PageSize);
    }

    private IQueryable<Student> GetFilteredByDroppedOutStatus(
        IQueryable<Student> students,
        StudentDroppedOutStatus droppedOutStatus
    )
    {
        students = droppedOutStatus switch
        {
            StudentDroppedOutStatus.All => students,
            StudentDroppedOutStatus.OnlyDroppedOut => students.Where(x => x.DroppedOutAt != null),
            StudentDroppedOutStatus.OnlyActive => students.Where(x => x.DroppedOutAt == null),
        };

        return students;
    }

    private IQueryable<Student> GetSortedBySortState(
        IQueryable<Student> students,
        SortState sortState
    )
    {
        students = sortState switch
        {
            SortState.FistNameAsc => students.OrderBy(s => s.FirstName),
            SortState.FirstNameDesc => students.OrderByDescending(s => s.FirstName),
            SortState.LastNameAsc => students.OrderBy(s => s.LastName),
            SortState.LastNameDesc => students.OrderByDescending(s => s.LastName),
            SortState.GroupAsc
                => students
                    .OrderBy(s => s.Group.CurrentSemester)
                    .ThenBy(s => s.Group.Speciality.Abbreavation)
                    .ThenBy(s => s.Group.SubGroup),
            SortState.GroupDesc => students.OrderByDescending(s => s.Group.CurrentSemester)
        };

        return students;
    }
}
