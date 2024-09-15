using Google.Protobuf.Collections;
using Mapster;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.API;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig.GlobalSettings.Default.UseDestinationValue(member =>
            member.SetterModifier == AccessModifier.None
            && member.Type.IsGenericType
            && member.Type.GetGenericTypeDefinition() == typeof(RepeatedField<>)
        );

        ConfigureGroups();
        ConfigureStudents();
        ConfigureSpecialities();
    }

    public static void ConfigureGroups()
    {
        TypeAdapterConfig<List<GroupShortInfoDto>, DeleteGroupsResponse>
            .NewConfig()
            .Map(dest => dest.Groups, src => src);

        TypeAdapterConfig<List<GroupShortInfoDto>, SoftDeleteGroupsResponse>
            .NewConfig()
            .Map(dest => dest.Groups, src => src);

        TypeAdapterConfig<List<GroupShortInfoDto>, RecoveryGroupsResponse>
            .NewConfig()
            .Map(dest => dest.Groups, src => src);

        TypeAdapterConfig<List<GroupShortInfoDto>, GraduateGroupsResponse>
            .NewConfig()
            .Map(dest => dest.Groups, src => src);

        TypeAdapterConfig<List<GroupShortInfoDto>, TransferGroupsToNextSemesterResponse>
            .NewConfig()
            .Map(dest => dest.Groups, src => src);

        TypeAdapterConfig<List<GroupShortInfoDto>, TransferGroupsToNextCourseResponse>
            .NewConfig()
            .Map(dest => dest.Groups, src => src);
    }

    public static void ConfigureStudents()
    {
        TypeAdapterConfig<List<StudentShortInfoDto>, DropOutStudentsResponse>
            .NewConfig()
            .Map(dest => dest.Students, src => src);

        TypeAdapterConfig<List<StudentShortInfoDto>, DeleteStudentsResponse>
            .NewConfig()
            .Map(dest => dest.Students, src => src);

        TypeAdapterConfig<List<StudentShortInfoDto>, SoftDeleteStudentsResponse>
            .NewConfig()
            .Map(dest => dest.Students, src => src);

        TypeAdapterConfig<List<StudentShortInfoDto>, RecoveryStudentsResponse>
            .NewConfig()
            .Map(dest => dest.Students, src => src);
    }

    public static void ConfigureSpecialities()
    {
        TypeAdapterConfig<List<SpecialityShortInfoDto>, DeleteSpecialitiesResponse>
            .NewConfig()
            .Map(dest => dest.Specialities, src => src);

        TypeAdapterConfig<List<SpecialityShortInfoDto>, SoftDeleteSpecialitiesResponse>
            .NewConfig()
            .Map(dest => dest.Specialities, src => src);

        TypeAdapterConfig<List<SpecialityShortInfoDto>, RecoverySpecialitiesResponse>
            .NewConfig()
            .Map(dest => dest.Specialities, src => src);
    }
}
