using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class TransferGroupsToNextCourse
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public TransferGroupsToNextCourse()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task TransferGroupsToNextCourse_ShouldBe_Success()
    {
        var speciality = _fixture.Build<Speciality>().With(x => x.DurationMonths, 48).Create();

        _fixture.Customize<Group>(x =>
            x.With(x => x.CurrentCourse, 2).With(x => x.Speciality, speciality)
        );

        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);

        var command = _fixture
            .Build<TransferGroupsToNextCourseCommand>()
            .With(x => x.IdGroups, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new TransferGroupsToNextCourseCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(command, default);

        _mockDbContext.Verify(x => x.BeginTransactionAsync(), Times.Once());

        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());

        groups.All(x => x.CurrentCourse == 3).Should().BeTrue();
    }

    [Fact]
    public async Task TransferGroupsToNextCourse_ShouldBe_GroupNotFoundException_WhenGroupsDoNotExist()
    {
        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([]);

        var command = _fixture
            .Build<TransferGroupsToNextCourseCommand>()
            .With(x => x.IdGroups, [123, 512, 46, 7])
            .Create();

        var handler = new TransferGroupsToNextCourseCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Fact]
    public async Task TransferGroupsToNextCourse_ShouldBe_GroupCourseOutOfRangeException_WhenGroupCourseOutOfRange()
    {
        var speciality = _fixture.Build<Speciality>().With(x => x.DurationMonths, 12).Create();

        _fixture.Customize<Group>(x =>
            x.With(x => x.CurrentCourse, 1).With(x => x.Speciality, speciality)
        );

        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);

        var command = _fixture
            .Build<TransferGroupsToNextCourseCommand>()
            .With(x => x.IdGroups, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new TransferGroupsToNextCourseCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<GroupCourseOutOfRangeException>();
    }

    [Fact]
    public async Task TransferGroupsToNextCourse_ShouldBe_CallRallback_WhenThrowException()
    {
        var speciality = _fixture.Build<Speciality>().With(x => x.DurationMonths, 48).Create();

        _fixture.Customize<Group>(x =>
            x.With(x => x.CurrentCourse, 2).With(x => x.Speciality, speciality)
        );

        _mockDbContext.Setup(x => x.CommitTransactionAsync()).Throws(new Exception());

        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);

        var command = _fixture
            .Build<TransferGroupsToNextCourseCommand>()
            .With(x => x.IdGroups, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new TransferGroupsToNextCourseCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
