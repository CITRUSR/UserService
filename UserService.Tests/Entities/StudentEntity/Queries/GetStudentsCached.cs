using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
using UserService.Application.Enums;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentsCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task GetStudentsCached_ShouldBe_Success_WithoutCache_WithValidatedQuery()
    {
        await SeedData();

        var query = CreateQuery();

        var StudentsRes = await Action(query);

        var key = CacheKeys.GetEntities<Student>(query.Page, query.PageSize);

        var cacheString = await CacheService.GetStringAsync(key);
        var StudentsFromCache = await CacheService.GetObjectAsync<PaginationList<Student>>(key);

        cacheString.Should().NotBeNullOrEmpty();
        StudentsFromCache.Should().BeEquivalentTo(StudentsRes);
    }

    [Fact]
    public async Task GetStudentsCached_ShouldBe_Success_WithCache_WithValidatedQuery()
    {
        await SeedData();

        var students = Context.Students;
        ;

        var query = CreateQuery();

        var paginationList = await PaginationList<Student>.CreateAsync(
            students,
            query.Page,
            query.PageSize
        );

        var key = CacheKeys.GetEntities<Student>(query.Page, query.PageSize);

        await CacheService.SetObjectAsync(key, paginationList);

        await Action(query);

        var cacheString = await CacheService.GetStringAsync(key);
        var studentsFromCache = await CacheService.GetObjectAsync<PaginationList<Student>>(key);

        cacheString.Should().NotBeNullOrEmpty();
        studentsFromCache.Should().BeEquivalentTo(paginationList);
    }

    [Fact]
    public async Task GetStudentsCached_ShouldBe_Success_WithoutValidatedQuery()
    {
        await SeedData();

        var query = CreateQuery(sortState: SortState.FistNameAsc);

        var key = CacheKeys.GetEntities<Student>(query.Page, query.PageSize);

        await Action(query);

        var cacheString = await CacheService.GetStringAsync(key);
        var StudentsFromCache = await CacheService.GetObjectAsync<PaginationList<Student>>(key);

        cacheString.Should().BeNullOrEmpty();
        StudentsFromCache.Should().BeNull();
    }

    private async Task SeedData()
    {
        var students = Fixture.CreateMany<Student>(10);

        await DbHelper.AddStudentsToContext([.. students]);
    }

    private GetStudentsQuery CreateQuery(
        int page = 1,
        int pageSize = 10,
        string searchString = "",
        SortState sortState = SortState.LastNameAsc,
        DeletedStatus deletedStatus = DeletedStatus.OnlyActive,
        StudentDroppedOutStatus droppedOutStatus = StudentDroppedOutStatus.OnlyActive
    )
    {
        return new GetStudentsQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchString = searchString,
            DeletedStatus = deletedStatus,
            SortState = sortState,
            DroppedOutStatus = droppedOutStatus,
        };
    }

    private async Task<PaginationList<Student>> Action(GetStudentsQuery query)
    {
        var handler = new GetStudentsQueryHandlerCached(
            new GetStudentsQueryHandler(Context),
            CacheService
        );

        return await handler.Handle(query, CancellationToken.None);
    }
}
