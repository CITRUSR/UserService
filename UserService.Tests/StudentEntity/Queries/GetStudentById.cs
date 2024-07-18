using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.StudentEntity.Queries;

public class GetStudentById : CommonTest
{
    [Fact]
    public async void GetStudentById_ShouldBe_Success()
    {
        var student = Fixture.Create<Student>();

        await Context.AddAsync(student);

        var query = new GetStudentByIdQuery(student.Id);
        var handler = new GetStudentByIdQueryHandler(Context);

        var foundStudent = await handler.Handle(query, CancellationToken.None);

        foundStudent.Should().BeEquivalentTo(student);
    }

    [Fact]
    public async void GetStudentById_ShouldBe_NotFoundException()
    {
        var query = Fixture.Create<GetStudentByIdQuery>();
        var handler = new GetStudentByIdQueryHandler(Context);

        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }
}