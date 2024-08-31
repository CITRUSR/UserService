using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class EditSpeciality
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;
    private readonly EditSpecialityCommand _command;

    public EditSpeciality()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
        _command = _fixture.Create<EditSpecialityCommand>();
    }

    [Fact]
    public async Task EditSpeciality_ShouldBe_Success()
    {
        var speciality = _fixture.Build<Speciality>().With(x => x.Id, _command.Id).Create();

        _mockDbContext
            .Setup(x =>
                x.Specialities.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(speciality);

        var handler = new EditSpecialityCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(_command, default);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        speciality.Abbreavation.Should().Be(_command.Abbrevation);
        speciality.Cost.Should().Be(_command.Cost);
        speciality.DurationMonths.Should().Be(_command.DurationMonths);
        speciality.Name.Should().Be(_command.Name);
        speciality.IsDeleted.Should().Be(_command.IsDeleted);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task EditSpeciality_ShouldBe_SpecialityNotFoundException_WhenSpecialityDoesNotExist()
    {
        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([new Speciality()]);

        var handler = new EditSpecialityCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(_command, default);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }
}
