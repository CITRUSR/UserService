using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroupById(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task GetGroupById_ShouldBe_Success()
    {
        var group = Fixture.Create<Group>();

        await AddGroupsToContext(group);

        var query = new GetGroupByIdQuery(group.Id);

        var groupRes = await Action(query);

        Context.Groups.FirstOrDefault(x => x.Id == group.Id).Should().BeEquivalentTo(groupRes);
    }

    [Fact]
    public async Task GetGroupById_ShouldBe_GroupNotFoundException()
    {
        var query = Fixture.Create<GetGroupByIdQuery>();

        Func<Task> act = async () => await Action(query);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    private async Task<Group> Action(GetGroupByIdQuery query)
    {
        var handler = new GetGroupByIdQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}
