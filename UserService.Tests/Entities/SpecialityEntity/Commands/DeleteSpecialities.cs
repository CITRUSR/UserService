using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpecialities;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class DeleteSpecialities
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public DeleteSpecialities()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteSpecialities_ShouldBe_Success()
    {
        var specialitites = _fixture.CreateMany<Speciality>(3);

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([.. specialitites]);

        var command = _fixture
            .Build<DeleteSpecialitiesCommand>()
            .With(x => x.SpecialitiesId, [.. specialitites.Select(x => x.Id)])
            .Create();

        var handler = new DeleteSpecialitiesCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(
            x =>
                x.Specialities.RemoveRange(
                    It.Is<List<Speciality>>(x =>
                        x.TrueForAll(z => command.SpecialitiesId.Contains(z.Id))
                    )
                ),
            Times.Once()
        );

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteSpecialities_ShouldBe_SpecialityNotFoundException_WhenSoecialitiesDoNotExist()
    {
        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([]);

        var command = _fixture
            .Build<DeleteSpecialitiesCommand>()
            .With(x => x.SpecialitiesId, [123, 512, 46])
            .Create();

        var handler = new DeleteSpecialitiesCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    [Fact]
    public async Task DeleteSpecialities_ShouldBe_CallRallback_WhenThrowException()
    {
        var specialities = _fixture.CreateMany<Speciality>(3);

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([.. specialities]);
        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var command = _fixture
            .Build<DeleteSpecialitiesCommand>()
            .With(x => x.SpecialitiesId, [.. specialities.Select(x => x.Id)])
            .Create();

        var handler = new DeleteSpecialitiesCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
