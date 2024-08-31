using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class GraduateGroups
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public GraduateGroups()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GraduateGroup_ShouldBe_Success()
    {
        _fixture.Customize<Group>(c => c.Without(x => x.GraduatedAt));

        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);

        var command = _fixture
            .Build<GraduateGroupsCommand>()
            .With(x => x.GroupsId, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new GraduateGroupsCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(x => x.BeginTransactionAsync(), Times.Once);

        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());

        groups.All(x => x.GraduatedAt.HasValue).Should().BeTrue();
    }

    [Fact]
    public async Task GraduateGroup_ShouldBe_GroupNotFoundException_WhenGroupsDoNotExist()
    {
        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([new Group(), new Group()]);

        var command = _fixture
            .Build<GraduateGroupsCommand>()
            .With(x => x.GroupsId, [123, 5236, 547])
            .Create();

        var handler = new GraduateGroupsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Fact]
    public async Task GraduateGroup_ShouldBe_GroupAlreadyGraduatedException_WhenGroupsAlreadyGraduated()
    {
        _fixture.Customize<Group>(c => c.With(x => x.GraduatedAt, DateTime.Now));

        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);

        var command = _fixture
            .Build<GraduateGroupsCommand>()
            .With(x => x.GroupsId, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new GraduateGroupsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupAlreadyGraduatedException>();
    }

    [Fact]
    public async Task GraduateGroup_ShouldBe_CallRallback_WhenThrowException()
    {
        _fixture.Customize<Group>(c => c.With(x => x.GraduatedAt, DateTime.Now));

        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);

        _mockDbContext.Setup(x => x.BeginTransactionAsync()).Throws(new Exception());

        var command = _fixture
            .Build<GraduateGroupsCommand>()
            .With(x => x.GroupsId, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new GraduateGroupsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
