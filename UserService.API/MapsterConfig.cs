using Mapster;
using UserService.Application.CQRS.GroupEntity.Responses;

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
    }
}
