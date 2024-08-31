using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class SoftDeleteGroups
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public SoftDeleteGroups()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task SoftDeleteGroups_ShouldBe_Success()
    {
        _fixture.Customize<Group>(x => x.With(x => x.IsDeleted, false));

        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);

        var command = _fixture
            .Build<SoftDeleteGroupsCommand>()
            .With(x => x.GroupsId, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new SoftDeleteGroupsCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(x => x.BeginTransactionAsync(), Times.Once);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());

        groups.All(x => x.IsDeleted).Should().BeTrue();
    }

    [Fact]
    public async Task SoftDeleteGroups_ShouldBe_GroupNotFoundException_WhenGroupsDoNotExist()
    {
        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([]);

        var command = _fixture
            .Build<SoftDeleteGroupsCommand>()
            .With(x => x.GroupsId, [123, 124, 523])
            .Create();

        var handler = new SoftDeleteGroupsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Fact]
    public async Task SoftDeleteGroups_ShouldBe_CallRallback_WhenThrowException()
    {
        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var command = _fixture
            .Build<SoftDeleteGroupsCommand>()
            .With(x => x.GroupsId, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new SoftDeleteGroupsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
