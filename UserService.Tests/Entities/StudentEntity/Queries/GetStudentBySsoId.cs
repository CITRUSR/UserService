using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentBySsoId(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void GetStudentBySsoId_ShouldBe_Success()
    {
        var student = Fixture.Create<Student>();

        await AddStudentsToContext(student);

        var query = new GetStudentBySsoIdQuery(student.SsoId);

        var handler = new GetStudentBySsoIdQueryHandler(Context);

        var studentRes = await handler.Handle(query, CancellationToken.None);

        studentRes.Should().NotBeNull();
        studentRes.Should().BeEquivalentTo(student);
    }

    [Fact]
    public async void GetStudentBySsoId_ShouldBe_StudentNotFoundException()
    {
        var query = new GetStudentBySsoIdQuery(Guid.NewGuid());

        var handler = new GetStudentBySsoIdQueryHandler(Context);

        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }
}