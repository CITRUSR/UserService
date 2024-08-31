using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class CreateGroup
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;
    private readonly CreateGroupCommand _command;

    public CreateGroup()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
        _command = _fixture.Create<CreateGroupCommand>();
    }

    [Fact]
    public async Task CreateGroup_ShouldBe_Success()
    {
        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([]);

        _mockDbContext
            .Setup(x =>
                x.Specialities.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Speciality());

        _mockDbContext
            .Setup(x => x.Teachers.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Teacher());

        var handler = new CreateGroupCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(_command, CancellationToken.None);

        _mockDbContext.Verify(
            x => x.Groups.AddAsync(It.IsAny<Group>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateGroup_ShouldBe_SpecialityNotFoundException_WhenSpecialityDoesNotExist()
    {
        _mockDbContext
            .Setup(x =>
                x.Specialities.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Speciality?)null);

        var handler = new CreateGroupCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    [Fact]
    public async Task CreateGroup_ShouldBe_TeacherNotFoundException_WhenCuratorDoesNotExist()
    {
        _mockDbContext
            .Setup(x =>
                x.Specialities.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Speciality());

        _mockDbContext
            .Setup(x => x.Teachers.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        var handler = new CreateGroupCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<TeacherNotFoundException>();
    }
}
