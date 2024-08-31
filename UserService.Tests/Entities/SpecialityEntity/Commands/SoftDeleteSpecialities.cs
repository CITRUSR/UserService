using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class SoftDeleteSpecialities
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public SoftDeleteSpecialities()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task SoftDeleteSpecialties_ShouldBe_Success()
    {
        _fixture.Customize<Speciality>(x => x.With(x => x.IsDeleted, false));

        var specialities = _fixture.CreateMany<Speciality>(3);

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([.. specialities]);

        var command = _fixture
            .Build<SoftDeleteSpecialitiesCommand>()
            .With(x => x.SpecialitiesId, [.. specialities.Select(x => x.Id)])
            .Create();

        var handler = new SoftDeleteSpecialitiesCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());

        specialities.All(x => x.IsDeleted).Should().BeTrue();
    }

    [Fact]
    public async Task SoftDeleteSpecialties_ShouldBe_SpecialitypNotFoundException_WhenSpecialitiesDoNotExist()
    {
        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([]);

        var command = _fixture
            .Build<SoftDeleteSpecialitiesCommand>()
            .With(x => x.SpecialitiesId, [123, 124, 523])
            .Create();

        var handler = new SoftDeleteSpecialitiesCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    [Fact]
    public async Task SoftDeleteSpecialties_ShouldBe_CallRallback_WhenThrowException()
    {
        var specialities = _fixture.CreateMany<Speciality>(3);

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([.. specialities]);

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var command = _fixture
            .Build<SoftDeleteSpecialitiesCommand>()
            .With(x => x.SpecialitiesId, [.. specialities.Select(x => x.Id)])
            .Create();

        var handler = new SoftDeleteSpecialitiesCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
