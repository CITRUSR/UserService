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
    }
}
