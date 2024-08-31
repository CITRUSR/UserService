using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class DeleteGroups
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public DeleteGroups()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteGroups_ShouldBe_Success()
    {
        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);

        var command = _fixture
            .Build<DeleteGroupsCommand>()
            .With(x => x.Ids, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new DeleteGroupsCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(
            x =>
                x.Groups.RemoveRange(
                    It.Is<List<Group>>(x => x.All(z => command.Ids.Contains(z.Id)))
                ),
            Times.Once()
        );

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteGroups_ShouldBe_GroupNotFoundException_WhenGroupsDoNotExist()
    {
        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([]);

        var command = _fixture
            .Build<DeleteGroupsCommand>()
            .With(x => x.Ids, [123, 512, 46])
            .Create();

        var handler = new DeleteGroupsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Fact]
    public async Task DeleteGroups_ShouldBe_CallRallback_WhenThrowException()
    {
        var groups = _fixture.CreateMany<Group>(3);

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([.. groups]);
        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var command = _fixture
            .Build<DeleteGroupsCommand>()
            .With(x => x.Ids, [.. groups.Select(x => x.Id)])
            .Create();

        var handler = new DeleteGroupsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
