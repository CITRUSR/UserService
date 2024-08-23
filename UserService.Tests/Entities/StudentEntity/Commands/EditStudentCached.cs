using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.EditStudent;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class EditStudentCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task EditStudentCached_ShouldBe_Success()
    {
        var student = Fixture.Create<Student>();

        await DbHelper.AddStudentsToContext(student);

        await CacheService.SetObjectAsync<Student>(
            CacheKeys.ById<Student, Guid>(student.Id),
            student
        );

        var command = Fixture
            .Build<EditStudentCommand>()
            .With(x => x.Id, student.Id)
            .With(x => x.FirstName, "asdasdasd")
            .With(x => x.LastName, "asdasdasd")
            .With(x => x.PatronymicName, "asdasdasd")
            .With(x => x.GroupId, student.GroupId)
            .Create();

        var handler = new EditStudentCommandHandlerCached(
            CacheService,
            new EditStudentCommandHandler(Context)
        );

        await handler.Handle(command, CancellationToken.None);

        var studentFromCache = await CacheService.GetObjectAsync<Student>(
            CacheKeys.ById<Student, Guid>(student.Id)
        );

        studentFromCache.Should().BeNull();
    }
}
