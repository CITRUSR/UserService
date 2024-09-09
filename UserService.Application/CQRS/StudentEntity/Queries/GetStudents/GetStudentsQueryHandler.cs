using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Application.Extensions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.StudentEntity.Queries.GetStudents;

public class GetStudentsQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<GetStudentsQuery, GetStudentsResponse>
{
    public async Task<GetStudentsResponse> Handle(
        GetStudentsQuery request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Student> students = DbContext.Students.Include(x => x.Group);

        students = students.FilterByDeletedStatus<Student>(
            request.DeletedStatus,
            st => st.IsDeleted
        );

        students = GetFilteredByDroppedOutStatus(students, request.DroppedOutStatus);

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            students = students.Where(x =>
                $"{x.FirstName}{x.LastName}{x.PatronymicName}{x.Group.CurrentCourse}-{x.Group.Speciality.Abbreviation}{x.Group.SubGroup}".Contains(
                    request.SearchString,
                    StringComparison.CurrentCultureIgnoreCase
                )
            );
        }

        students = GetSortedBySortState(students, request.SortState);

        var pagList = await PaginationList<Student>.CreateAsync(
            students,
            request.Page,
            request.PageSize
        );

        return pagList.Adapt<GetStudentsResponse>();
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
                    .OrderBy(s => s.Group.CurrentCourse)
                    .ThenBy(s => s.Group.Speciality.Abbreviation)
                    .ThenBy(s => s.Group.SubGroup),
            SortState.GroupDesc
                => students
                    .OrderByDescending(s => s.Group.CurrentCourse)
                    .ThenByDescending(s => s.Group.Speciality.Abbreviation)
                    .ThenByDescending(s => s.Group.SubGroup),
        };

        return students;
    }
}
