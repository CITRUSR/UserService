using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class EditGroup
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;
    private readonly EditGroupCommand _command;

    public EditGroup()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
        _command = _fixture.Create<EditGroupCommand>();
    }

    [Fact]
    public async Task EditGroup_ShouldBe_Success()
    {
        var group = _fixture.Build<Group>().With(x => x.Id, _command.Id).Create();

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([group]);

        _mockDbContext
            .Setup(x =>
                x.Specialities.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Speciality());

        _mockDbContext
            .Setup(x => x.Teachers.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Teacher());

        var handler = new EditGroupCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(_command, default);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        group.CuratorId.Should().Be(_command.CuratorId);
        group.SpecialityId.Should().Be(_command.SpecialityId);
        group.CurrentCourse.Should().Be(_command.CurrentCourse);
        group.CurrentSemester.Should().Be(_command.CurrentSemester);
        group.IsDeleted.Should().Be(_command.IsDeleted);
        group.SubGroup.Should().Be(_command.SubGroup);
    }

    [Fact]
    public async Task EditGroup_ShouldBe_GroupNotFoundException_WhenGroupDoesNotExist()
    {
        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([new Group()]);

        var handler = new EditGroupCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(_command, default);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Fact]
    public async Task EditGroup_ShouldBe_SpecialityNotFoundException_WhenSpecialityDoesNotExist()
    {
        var group = _fixture.Build<Group>().With(x => x.Id, _command.Id).Create();

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([group]);

        _mockDbContext
            .Setup(x =>
                x.Specialities.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Speciality?)null);

        var handler = new EditGroupCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(_command, default);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    [Fact]
    public async Task EditGroup_ShouldBe_TeacherNotFoundException_WhenCuratorDoesNotExist()
    {
        var group = _fixture.Build<Group>().With(x => x.Id, _command.Id).Create();

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([group]);

        _mockDbContext
            .Setup(x =>
                x.Specialities.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Speciality());

        _mockDbContext
            .Setup(x => x.Teachers.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        var handler = new EditGroupCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(_command, default);

        await act.Should().ThrowAsync<TeacherNotFoundException>();
    }
}
