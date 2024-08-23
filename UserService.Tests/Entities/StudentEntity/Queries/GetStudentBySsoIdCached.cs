using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentBySsoIdCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task GetStudentById_ShouldBe_Success_WithoutCache()
    {
        var student = Fixture.Create<Student>();

        await DbHelper.AddStudentsToContext(student);

        var query = new GetStudentBySsoIdQuery(student.SsoId);

        var key = CacheKeys.ById<Student, Guid>(student.SsoId);

        var studentFromCacheBeforeAction = await CacheService.GetObjectAsync<Student>(key);

        var studentRes = await Action(query);

        var studentFromCache = await CacheService.GetObjectAsync<Student>(key);

        studentFromCacheBeforeAction.Should().BeNull();

        studentRes
            .Should()
            .BeEquivalentTo(studentFromCache, options => options.Excluding(x => x.Group));
    }

    [Fact]
    public async Task GetStudentById_ShouldBe_Success_WithCache()
    {
        var student = Fixture.Create<Student>();

        await DbHelper.AddStudentsToContext(student);

        var key = CacheKeys.ById<Student, Guid>(student.Id);

        await CacheService.SetObjectAsync<Student>(key, student);

        var query = new GetStudentBySsoIdQuery(student.Id);

        var studentRes = await Action(query);

        var studentFromCache = await CacheService.GetObjectAsync<Student>(key);

        studentRes
            .Should()
            .BeEquivalentTo(studentFromCache, options => options.Excluding(x => x.Group));
    }

    private async Task<Student> Action(GetStudentBySsoIdQuery query)
    {
        var handler = new GetStudentBySsoIdQueryHandlerCached(
            new GetStudentBySsoIdQueryHandler(Context),
            CacheService
        );

        return await handler.Handle(query, CancellationToken.None);
    }
}
