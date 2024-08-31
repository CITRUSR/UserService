using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class CreateSpeciality
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public CreateSpeciality()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CreateSpeciality_ShouldBe_Success()
    {
        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([]);

        var command = _fixture.Create<CreateSpecialityCommand>();

        var handler = new CreateSpecialityCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(
            x => x.Specialities.AddAsync(It.IsAny<Speciality>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        result.Should().NotBeNull();
    }
}
