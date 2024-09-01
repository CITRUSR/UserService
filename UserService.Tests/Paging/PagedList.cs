using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Paging;
using UserService.Domain.Entities;

namespace UserService.Tests.Paging;

public class PagedList
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private IFixture _fixture;

    public PagedList()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task PagedList_ShouldBe_Success()
    {
        var specialities = _fixture.CreateMany<Speciality>(12).ToList();

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([.. specialities]);

        //create async query with linq and ef core
        var query = _mockDbContext.Object.Specialities.Where(x =>
            specialities.Select(x => x.Id).Contains(x.Id)
        );

        var pagedList = await PaginationList<Speciality>.CreateAsync(query, 2, 10);

        pagedList.Items.Count.Should().Be(2);
        pagedList
            .Items.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(specialities.Skip(10).Take(2).Select(x => x.Id));
        pagedList.MaxPage.Should().Be(2);
        pagedList.TotalCount.Should().Be(12);
    }
}
