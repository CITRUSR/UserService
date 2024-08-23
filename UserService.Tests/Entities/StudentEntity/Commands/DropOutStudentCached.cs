using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DropOutStudentCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task DropOutStudentCached_ShouldBe_Success()
    {
        var student = Fixture.Create<Student>();

        await DbHelper.AddStudentsToContext(student);

        await CacheService.SetObjectAsync<Student>(
            CacheKeys.ById<Student, Guid>(student.Id),
            student
        );

        var command = Fixture.Build<DropOutStudentCommand>().With(x => x.Id, student.Id).Create();

        var handler = new DropOutStudentCommandHandlerCached(
            new DropOutStudentCommandHandler(Context),
            CacheService
        );

        var id = await handler.Handle(command, CancellationToken.None);

        var studentFromCache = await CacheService.GetObjectAsync<Student>(
            CacheKeys.ById<Student, Guid>(id)
        );

        studentFromCache.Should().BeNull();
    }
}
