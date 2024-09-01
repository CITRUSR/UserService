using FluentAssertions;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentById
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public GetStudentById()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetStudentById_ShouldBe_Success()
    {
        var student = _fixture.Create<Student>();

        _mockDbContext
            .Setup(x => x.Students.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var query = new GetStudentByIdQuery(student.Id);

        var hanlder = new GetStudentByIdQueryHandler(_mockDbContext.Object);

        var result = await hanlder.Handle(query, default);

        result.Should().NotBeNull();
        result.Id.Should().Be(student.Id);
    }

    [Fact]
    public async Task GetStudentById_ShouldBe_StudentNotFoundException_WhenStudentDoesNotExist()
    {
        _mockDbContext
            .Setup(x => x.Students.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        var query = new GetStudentByIdQuery(Guid.NewGuid());

        var hanlder = new GetStudentByIdQueryHandler(_mockDbContext.Object);

        Func<Task> act = async () => await hanlder.Handle(query, default);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }
}
