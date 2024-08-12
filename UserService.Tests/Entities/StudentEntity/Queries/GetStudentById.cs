using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentById(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task GetStudentById_ShouldBe_Success()
    {
        var student = Fixture.Create<Student>();

        await AddStudentsToContext(student);

        var query = new GetStudentByIdQuery(student.Id);
        var handler = new GetStudentByIdQueryHandler(Context);

        var foundStudent = await handler.Handle(query, CancellationToken.None);

        foundStudent.Should().BeEquivalentTo(student);
    }

    [Fact]
    public async Task GetStudentById_ShouldBe_NotFoundException()
    {
        var query = Fixture.Create<GetStudentByIdQuery>();
        var handler = new GetStudentByIdQueryHandler(Context);

        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }
}
