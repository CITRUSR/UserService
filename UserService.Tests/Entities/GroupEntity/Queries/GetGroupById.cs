using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroupById
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public GetGroupById()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetGroupById_ShouldBe_Success()
    {
        var group = _fixture.Create<Group>();

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([group]);

        var query = new GetGroupByIdQuery(group.Id);

        var handler = new GetGroupByIdQueryHandler(_mockDbContext.Object);

        var result = await handler.Handle(query, default);

        result.Should().NotBeNull();
        result.Id.Should().Be(group.Id);
    }

    [Fact]
    public async Task GetGroupById_ShouldBe_GroupNotFoundException()
    {
        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([]);

        var query = new GetGroupByIdQuery(125125);

        var handler = new GetGroupByIdQueryHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(query, default);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}
