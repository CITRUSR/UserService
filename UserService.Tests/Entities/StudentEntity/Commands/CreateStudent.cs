using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class CreateStudent
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;
    private readonly CreateStudentCommand _command;

    public CreateStudent()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
        _command = _fixture.Create<CreateStudentCommand>();
    }

    [Fact]
    public async Task CreateStudent_ShouldBe_Success()
    {
        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([]);

        _mockDbContext
            .Setup(x => x.Groups.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Group());

        var handler = new CreateStudentCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(_command, CancellationToken.None);

        _mockDbContext.Verify(
            x => x.Students.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateStudent_ShouldBe_GroupNotFoundException_WhenGroupDoesNotExist()
    {
        _mockDbContext
            .Setup(x => x.Groups.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var handler = new CreateStudentCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}
