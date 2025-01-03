using FluentAssertions;
using MediatR;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class EditSpecialityCached
{
    private readonly Mock<IAppDbContext> _mockAppDbContext;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<EditSpecialityCommand, SpecialityShortInfoDto>
    > _mockHandler;
    private readonly EditSpecialityCommand _command;
    private readonly IFixture _fixture;

    public EditSpecialityCached()
    {
        _mockAppDbContext = new Mock<IAppDbContext>();
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<EditSpecialityCommand, SpecialityShortInfoDto>>();
        _fixture = new Fixture();
        _command = _fixture.Create<EditSpecialityCommand>();
    }

    [Fact]
    public async Task EditSpecialityCached_ShouldBe_Success()
    {
        var speciality = _fixture
            .Build<SpecialityShortInfoDto>()
            .With(x => x.Id, _command.Id)
            .Create();

        var groups = _fixture
            .Build<Group>()
            .With(x => x.SpecialityId, speciality.Id)
            .CreateMany(5)
            .ToList();

        _mockAppDbContext.Setup(x => x.Groups).ReturnsDbSet(groups);

        _mockHandler
            .Setup(x => x.Handle(It.IsAny<EditSpecialityCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(speciality);

        var handler = new EditSpecialityCommandHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object,
            _mockAppDbContext.Object
        );

        var result = await handler.Handle(_command, default);

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Speciality, int>(_command.Id))),
                    default
                ),
            Times.Once()
        );

        foreach (var group in groups)
        {
            _mockCacheService.Verify(
                x =>
                    x.RemoveAsync(
                        CacheKeys.ById<Group, int>(group.Id),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once()
            );
        }

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.GetEntities<Speciality>())),
                    default
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
