using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DeleteStudent
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public DeleteStudent()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteStudent_ShouldBe_Success()
    {
        var student = _fixture.Create<Student>();

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([student]);

        _mockDbContext
            .Setup(x => x.Students.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _mockDbContext
            .Setup(x => x.Groups.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Group());

        var command = _fixture.Build<DeleteStudentCommand>().With(x => x.Id, student.Id).Create();

        var handler = new DeleteStudentCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(
            x => x.Students.Remove(It.Is<Student>(x => x.Id == student.Id)),
            Times.Once()
        );

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task DeleteStudent_ShouldBe_StudentNotFoundException_WhenStudentDoesNotExist()
    {
        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([]);

        var command = _fixture
            .Build<DeleteStudentCommand>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();

        var handler = new DeleteStudentCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }
}
