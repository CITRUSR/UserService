using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstraction;
using UserService.Application.Common.Behaviors;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;
using UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;
using UserService.Domain.Entities;

namespace UserService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
        );

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        DecorateHandlersToCacheVersion(services);

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }

    private static void DecorateHandlersToCacheVersion(IServiceCollection services)
    {
        DecorateGroupsHandlersToCacheVersion(services);
        DecorateSpecialitiesHandlersToCacheVersion(services);
    }

    private static void DecorateGroupsHandlersToCacheVersion(IServiceCollection services)
    {
        services.Decorate<
            IRequestHandler<GetGroupByIdQuery, Group>,
            GetGroupByIdQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetGroupsQuery, PaginationList<Group>>,
            GetGroupsQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<DeleteGroupsCommand, List<Group>>,
            DeleteGroupsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<SoftDeleteGroupsCommand, List<Group>>,
            SoftDeleteGroupsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<EditGroupCommand, Group>,
            EditGroupCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GraduateGroupsCommand, List<Group>>,
            GraduateGroupsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<TransferGroupsToNextCourseCommand, List<Group>>,
            TransferGroupsToNextCourseCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<TransferGroupsToNextSemesterCommand, List<Group>>,
            TransferGroupsToNextSemesterCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<CreateGroupCommand, Group>,
            CreateGroupCommandHandlerCached
        >();
    }

    private static void DecorateSpecialitiesHandlersToCacheVersion(IServiceCollection services)
    {
        services.Decorate<
            IRequestHandler<GetSpecialityByIdQuery, Speciality>,
            GetSpecialityByIdQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetSpecialitiesQuery, PaginationList<Speciality>>,
            GetSpecialitiesQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<DeleteSpecialityCommand, List<Speciality>>,
            DeleteSpecialityCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<SoftDeleteSpecialitiesCommand, List<Speciality>>,
            SoftDeleteSpecialitiesCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<EditSpecialityCommand, Speciality>,
            EditSpecialityCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<CreateSpecialityCommand, Speciality>,
            CreateSpecialityCommandHandlerCached
        >();
    }
}
