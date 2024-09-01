using Mapster;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Group, GroupShortInfoDto>
            .NewConfig()
            .Map(dest => dest.GroupName, src => src.ToString());
    }
}
