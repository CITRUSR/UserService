using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstraction;
using UserService.Application.Common.Behaviors;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Domain.Entities;

namespace UserService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        DecorateHandlersToCacheVersion(services);

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }

    private static void DecorateHandlersToCacheVersion(IServiceCollection services)
    {
        services.Decorate<IRequestHandler<GetGroupByIdQuery, Group>, GetGroupByIdQueryHandlerCached>();
        services.Decorate<IRequestHandler<GetGroupsQuery, PaginationList<Group>>, GetGroupsQueryHandlerCached>();
        services.Decorate<IRequestHandler<DeleteGroupCommand, int>, DeleteGroupCommandHandlerCached>();
        services.Decorate<IRequestHandler<EditGroupCommand, Group>, EditGroupCommandHandlerCached>();
    }
}