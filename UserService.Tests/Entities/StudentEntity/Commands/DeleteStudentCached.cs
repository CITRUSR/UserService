using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DeleteStudentCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task DeleteStudentCached_ShouldBe_Success()
    {
        var student = Fixture.Create<Student>();

        await DbHelper.AddStudentsToContext(student);

        await CacheService.SetObjectAsync<Student>(
            CacheKeys.ById<Student, Guid>(student.Id),
            student
        );

        var command = new DeleteStudentCommand(student.Id);

        var handler = new DeleteStudentCommandHandlerCached(
            CacheService,
            new DeleteStudentCommandHandler(Context)
        );

        await handler.Handle(command, CancellationToken.None);

        var studentFromCache = await CacheService.GetObjectAsync<Student>(
            CacheKeys.ById<Student, Guid>(student.Id)
        );

        studentFromCache.Should().BeNull();
    }
}
