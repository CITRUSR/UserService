﻿using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroupByIdCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<GetGroupByIdQuery, Group>> _mockHandler;
    private readonly IFixture _fixture;

    public GetGroupByIdCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GetGroupByIdQuery, Group>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetGroupById_ShouldBe_Success()
    {
        var group = _fixture.Create<Group>();

        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<Group>(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Group, int>(group.Id))),
                    It.IsAny<Func<Task<Group>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(group);

        var query = new GetGroupByIdQuery(group.Id);

        var handler = new GetGroupByIdQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var result = await handler.Handle(query, default);

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<Group>(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Group, int>(group.Id))),
                    It.IsAny<Func<Task<Group>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
