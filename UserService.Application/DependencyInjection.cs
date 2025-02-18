﻿using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Common.Behaviors;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;
using UserService.Application.CQRS.GroupEntity.Commands.RecoveryGroups;
using UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.RecoverySpecialities;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;
using UserService.Application.CQRS.StudentEntity.Commands.EditStudent;
using UserService.Application.CQRS.StudentEntity.Commands.RecoveryStudents;
using UserService.Application.CQRS.StudentEntity.Commands.SoftDeleteStudents;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentsByGroupId;
using UserService.Application.CQRS.StudentEntity.Responses;
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

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        MapsterConfig.Configure();

        DecorateHandlersToCacheVersion(services);

        return services;
    }

    private static void DecorateHandlersToCacheVersion(IServiceCollection services)
    {
        DecorateGroupsHandlersToCacheVersion(services);
        DecorateSpecialitiesHandlersToCacheVersion(services);
        DecorateStudentsHandlerToCacheVersion(services);
    }

    private static void DecorateGroupsHandlersToCacheVersion(IServiceCollection services)
    {
        services.Decorate<
            IRequestHandler<GetGroupByIdQuery, GroupDto>,
            GetGroupByIdQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetGroupsQuery, GetGroupsResponse>,
            GetGroupsQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<DeleteGroupsCommand, List<GroupShortInfoDto>>,
            DeleteGroupsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<SoftDeleteGroupsCommand, List<GroupShortInfoDto>>,
            SoftDeleteGroupsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<RecoveryGroupsCommand, List<GroupShortInfoDto>>,
            RecoveryGroupsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<EditGroupCommand, GroupShortInfoDto>,
            EditGroupCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GraduateGroupsCommand, List<GroupShortInfoDto>>,
            GraduateGroupsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<TransferGroupsToNextCourseCommand, List<GroupShortInfoDto>>,
            TransferGroupsToNextCourseCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<TransferGroupsToNextSemesterCommand, List<GroupShortInfoDto>>,
            TransferGroupsToNextSemesterCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<CreateGroupCommand, GroupShortInfoDto>,
            CreateGroupCommandHandlerCached
        >();
    }

    private static void DecorateSpecialitiesHandlersToCacheVersion(IServiceCollection services)
    {
        services.Decorate<
            IRequestHandler<GetSpecialityByIdQuery, SpecialityDto>,
            GetSpecialityByIdQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetSpecialitiesQuery, GetSpecialitiesResponse>,
            GetSpecialitiesQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<DeleteSpecialitiesCommand, List<SpecialityShortInfoDto>>,
            DeleteSpecialitiesCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<SoftDeleteSpecialitiesCommand, List<SpecialityShortInfoDto>>,
            SoftDeleteSpecialitiesCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<RecoverySpecialitiesCommand, List<SpecialityShortInfoDto>>,
            RecoverySpecialitiesCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<EditSpecialityCommand, SpecialityShortInfoDto>,
            EditSpecialityCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<CreateSpecialityCommand, SpecialityShortInfoDto>,
            CreateSpecialityCommandHandlerCached
        >();
    }

    private static void DecorateStudentsHandlerToCacheVersion(IServiceCollection services)
    {
        services.Decorate<
            IRequestHandler<CreateStudentCommand, StudentShortInfoDto>,
            CreateStudentCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<DeleteStudentsCommand, List<StudentShortInfoDto>>,
            DeleteStudentsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<SoftDeleteStudentsCommand, List<StudentShortInfoDto>>,
            SoftDeleteStudentsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<DropOutStudentsCommand, List<StudentShortInfoDto>>,
            DropOutStudentsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<EditStudentCommand, StudentShortInfoDto>,
            EditStudentCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetStudentByIdQuery, StudentDto>,
            GetStudentByIdQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetStudentBySsoIdQuery, StudentDto>,
            GetStudentBySsoIdQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetStudentsByGroupIdQuery, List<StudentViewModel>>,
            GetStudentsByGroupIdQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetStudentsQuery, GetStudentsResponse>,
            GetStudentsQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<RecoveryStudentsCommand, List<StudentShortInfoDto>>,
            RecoveryStudentsCommandHandlerCached
        >();
    }
}
