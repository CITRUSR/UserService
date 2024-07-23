﻿using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Common.Behaviors;
using UserService.Application.Common.Paging;
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

        return services;
    }

    private static void DecorateHandlersToCacheVersion(IServiceCollection services)
    {
        services.Decorate<IRequestHandler<GetGroupByIdQuery, Group>, GetGroupsByIdQueryHandlerCached>();
        services.Decorate<IRequestHandler<GetGroupsQuery, PaginationList<Group>>, GetGroupsQueryHandlerCached>();
    }
}