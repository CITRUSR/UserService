using Mapster;
using UserService.Application.Common.Paging;
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
        TypeAdapterConfig<Group, GroupViewModel>
            .NewConfig()
            .Map(dest => dest.StudentCount, src => src.Students.Count)
            .Map(dest => dest.CuratorFirstName, src => src.Curator.FirstName)
            .Map(dest => dest.CuratorLastName, src => src.Curator.LastName)
            .Map(dest => dest.CuratorPatronymicName, src => src.Curator.PatronymicName)
            .Map(dest => dest.GroupName, src => src.ToString());
        TypeAdapterConfig<PaginationList<Group>, GetGroupsResponse>
            .NewConfig()
            .Map(dest => dest.LastPage, src => src.MaxPage)
            .Map(dest => dest.Groups, src => src.Items.Adapt<List<GroupViewModel>>());
    }
}
