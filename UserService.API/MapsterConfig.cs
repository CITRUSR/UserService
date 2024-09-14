using Mapster;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Application.CQRS.StudentEntity.Responses;

namespace UserService.API;

public static class MapsterConfig
{
    public static void Configure()
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

        TypeAdapterConfig<List<StudentShortInfoDto>, DropOutStudentsResponse>
            .NewConfig()
            .Map(dest => dest.Students, src => src);

        TypeAdapterConfig<List<StudentShortInfoDto>, DeleteStudentsResponse>
            .NewConfig()
            .Map(dest => dest.Students, src => src);

        TypeAdapterConfig<List<StudentShortInfoDto>, RecoveryStudentsResponse>
            .NewConfig()
            .Map(dest => dest.Students, src => src);
    }
}
