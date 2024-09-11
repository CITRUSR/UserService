using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Commands.RecoverySpecialities;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class RecoverySpecialities
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public RecoverySpecialities()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task RecoverySpecialities_ShouldBe_Success()
    {
        _fixture.Customize<Speciality>(x => x.With(x => x.IsDeleted, true));

        var specialities = _fixture.CreateMany<Speciality>(3);

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([.. specialities]);

        var command = _fixture
            .Build<RecoverySpecialitiesCommand>()
            .With(x => x.SpecialityIds, [.. specialities.Select(x => x.Id)])
            .Create();

        var handler = new RecoverySpecialitiesCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(x => x.BeginTransactionAsync(), Times.Once);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());

        specialities.All(x => x.IsDeleted).Should().BeFalse();
    }

    [Fact]
    public async Task RecoverySpecialities_ShouldBe_SpecialityNotFoundException()
    {
        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([]);

        var command = _fixture
            .Build<RecoverySpecialitiesCommand>()
            .With(x => x.SpecialityIds, [12, 6523, 2,])
            .Create();

        var handler = new RecoverySpecialitiesCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    [Fact]
    public async Task RecoveryGroups_ShouldBe_CallRallback_WhenThrowException()
    {
        var specialities = _fixture.CreateMany<Speciality>(3);

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([.. specialities]);

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var command = _fixture
            .Build<RecoverySpecialitiesCommand>()
            .With(x => x.SpecialityIds, [.. specialities.Select(x => x.Id)])
            .Create();

        var handler = new RecoverySpecialitiesCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
