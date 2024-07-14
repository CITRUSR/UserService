using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.Group.Queries.GetGroupById;
using UserService.Tests.Common;

namespace UserService.Tests.Group.Queries;

public class GetGroupById : CommonTest
{
    [Fact]
    public async void GetGroupById_ShouldBe_Success()
    {
        var group = Fixture.Create<Domain.Entities.Group>();

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

    private async Task<Domain.Entities.Group> Action(GetGroupByIdQuery query)
    {
        var handler = new GetGroupByIdQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}