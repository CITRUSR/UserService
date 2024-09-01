using FluentAssertions;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Queries;

public class GetSpecialityById
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public GetSpecialityById()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetSpecialityById_ShouldBe_Success()
    {
        var speciality = _fixture.Create<Speciality>();

        _mockDbContext
            .Setup(x =>
                x.Specialities.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(speciality);

        var query = new GetSpecialityByIdQuery(speciality.Id);

        var handler = new GetSpecialityByIdQueryHandler(_mockDbContext.Object);

        var result = await handler.Handle(query, default);

        result.Should().NotBeNull();
        result.Id.Should().Be(speciality.Id);
    }

    [Fact]
    public async Task GetSpecialityById_ShouldBe_SpecialityNotFoundException()
    {
        _mockDbContext
            .Setup(x =>
                x.Specialities.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Speciality?)null);

        var query = new GetSpecialityByIdQuery(125125);

        var handler = new GetSpecialityByIdQueryHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(query, default);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }
}
