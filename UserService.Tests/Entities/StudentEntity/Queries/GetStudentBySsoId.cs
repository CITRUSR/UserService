using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentBySsoId
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public GetStudentBySsoId()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetStudentBySsoId_ShouldBe_Success()
    {
        var student = _fixture.Create<Student>();

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([student]);

        var query = new GetStudentBySsoIdQuery(student.SsoId);

        var hanlder = new GetStudentBySsoIdQueryHandler(_mockDbContext.Object);

        var result = await hanlder.Handle(query, default);

        result.Should().NotBeNull();
        result.SsoId.Should().Be(student.SsoId);
    }

    [Fact]
    public async Task GetStudentBySsoId_ShouldBe_StudentNotFoundException_WhenStudentDoesNotExist()
    {
        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([]);

        var query = new GetStudentBySsoIdQuery(Guid.NewGuid());

        var hanlder = new GetStudentBySsoIdQueryHandler(_mockDbContext.Object);

        Func<Task> act = async () => await hanlder.Handle(query, default);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }
}
