using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.GroupEntity.Queries;

public class GetGroupById : CommonTest
{
    [Fact]
    public async void GetGroupById_ShouldBe_Success()
    {
        var group = Fixture.Create<Group>();

        await Context.Groups.AddAsync(group);
        await Context.SaveChangesAsync();

        var query = new GetGroupByIdQuery(group.Id);

        var groupRes = await Action(query);

        Context.Groups.FirstOrDefault(x => x.Id == group.Id).Should().BeEquivalentTo(groupRes);
    }

    [Fact]
    public async void GetGroupById_ShouldBe_GroupNotFoundException()
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