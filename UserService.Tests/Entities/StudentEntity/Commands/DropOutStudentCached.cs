using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DropOutStudentCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<DropOutStudentCommand, StudentShortInfoDto>> _mockHandler;
    private readonly IFixture _fixture;

    public DropOutStudentCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<DropOutStudentCommand, StudentShortInfoDto>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DropOutStudentCached_ShouldBe_Success()
    {
        var student = _fixture.Create<StudentShortInfoDto>();

        _mockHandler
            .Setup(x => x.Handle(It.IsAny<DropOutStudentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var command = new DropOutStudentCommand(student.Id, DateTime.Now);

        var handler = new DropOutStudentCommandHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var result = await handler.Handle(command, default);

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Student, Guid>(student.Id))),
                    It.IsAny<CancellationToken>()
                ),
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
