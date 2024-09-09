using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class CreateStudentCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<CreateStudentCommand, StudentShortInfoDto>> _mockHandler;
    private readonly IFixture _fixture;

    public CreateStudentCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<CreateStudentCommand, StudentShortInfoDto>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CreateStudentCached_ShouldBe_Success()
    {
        var student = _fixture.Create<StudentShortInfoDto>();

        _mockHandler
            .Setup(x => x.Handle(It.IsAny<CreateStudentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var command = _fixture.Create<CreateStudentCommand>();

        var handler = new CreateStudentCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(command, default);

        _mockHandler.Verify(
            x => x.Handle(It.IsAny<CreateStudentCommand>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.GetEntities<Student>())),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
