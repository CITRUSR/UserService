using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentByIdCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task GetStudentById_ShouldBe_Success_WithoutCache()
    {
        var student = Fixture.Create<Student>();

        await DbHelper.AddStudentsToContext(student);

        var query = new GetStudentByIdQuery(student.Id);

        var handler = new GetStudentByIdQueryHandlerCached(
            CacheService,
            new GetStudentByIdQueryHandler(Context)
        );

        var key = CacheKeys.ById<Student, Guid>(student.Id);

        var studentFromCacheBeforeAction = await CacheService.GetObjectAsync<Student>(key);

        var studentRes = await handler.Handle(query, CancellationToken.None);

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

        var query = new GetStudentByIdQuery(student.Id);

        var handler = new GetStudentByIdQueryHandlerCached(
            CacheService,
            new GetStudentByIdQueryHandler(Context)
        );

        var studentRes = await handler.Handle(query, CancellationToken.None);

        var studentFromCache = await CacheService.GetObjectAsync<Student>(key);

        studentRes
            .Should()
            .BeEquivalentTo(studentFromCache, options => options.Excluding(x => x.Group));
    }
}
