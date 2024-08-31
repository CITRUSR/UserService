using FluentAssertions;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.EditStudent;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class EditStudent
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public EditStudent()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task EditStudent_ShouldBe_Success()
    {
        var student = _fixture.Create<Student>();

        _mockDbContext
            .Setup(x => x.Students.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var group = _fixture.Create<Group>();

        _mockDbContext
            .Setup(x => x.Groups.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var command = _fixture
            .Build<EditStudentCommand>()
            .With(x => x.Id, student.Id)
            .With(x => x.GroupId, group.Id)
            .Create();

        var handler = new EditStudentCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(command, default);

        student.GroupId.Should().Be(command.GroupId);
        student.FirstName.Should().Be(command.FirstName);
        student.LastName.Should().Be(command.LastName);
        student.PatronymicName.Should().Be(command.PatronymicName);
        student.IsDeleted.Should().Be(command.IsDeleted);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task EditStudent_ShouldBe_StudentNotFoundException_WhenStudentDoesNotExist()
    {
        _mockDbContext
            .Setup(x => x.Students.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        var command = _fixture.Create<EditStudentCommand>();

        var handler = new EditStudentCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }

    [Fact]
    public async Task EditStudent_ShouldBe_GroupNotFoundException_WhenGroupDoesNotExist()
    {
        var student = _fixture.Create<Student>();

        _mockDbContext
            .Setup(x => x.Students.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _mockDbContext
            .Setup(x => x.Groups.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var command = _fixture.Build<EditStudentCommand>().With(x => x.Id, student.Id).Create();

        var handler = new EditStudentCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}
