using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Common.Behaviors;
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
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;
using UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;
using UserService.Application.CQRS.StudentEntity.Commands.EditStudent;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
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
            IRequestHandler<DropOutStudentsCommand, List<StudentShortInfoDto>>,
            DropOutStudentsCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<EditStudentCommand, Student>,
            EditStudentCommandHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetStudentByIdQuery, Student>,
            GetStudentByIdQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetStudentBySsoIdQuery, Student>,
            GetStudentBySsoIdQueryHandlerCached
        >();
        services.Decorate<
            IRequestHandler<GetStudentsQuery, PaginationList<Student>>,
            GetStudentsQueryHandlerCached
        >();
    }
}
