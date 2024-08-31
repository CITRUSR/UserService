using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DropOutStudent
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public DropOutStudent()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DropOutStudent_ShouldBe_Success()
    {
        var student = _fixture.Build<Student>().Without(x => x.DroppedOutAt).Create();

        _mockDbContext
            .Setup(x => x.Students.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var command = new DropOutStudentCommand(student.Id, DateTime.Now);

        var handler = new DropOutStudentCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(command, default);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        student.DroppedOutAt.Should().NotBeNull();

        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DropOutStudent_ShouldBe_StudentNotFoundException_WhenStudentDoesNotExist()
    {
        var student = _fixture.Build<Student>().Without(x => x.DroppedOutAt).Create();

        _mockDbContext
            .Setup(x => x.Students.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        var command = new DropOutStudentCommand(student.Id, DateTime.Now);

        var handler = new DropOutStudentCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }
}
