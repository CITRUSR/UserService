using Google.Protobuf.WellKnownTypes;
using UserService.Domain.Entities;

namespace UserService.API.Mappers;

public class GroupMapper : IMapper<Group, GroupModel>
{
    public GroupModel Map(Group from)
    {
        return new GroupModel
        {
            Id = from.Id,
            CuratorId = from.CuratorId.ToString(),
            CurrentCourse = from.CurrentCourse,
            CurrentSemester = from.CurrentSemester,
            StartedAt = from.StartedAt.ToTimestamp(),
            SubGroup = from.SubGroup,
            SpecialityId = from.SpecialityId,
            GraduatedAt = from.GraduatedAt.Value.ToTimestamp(),
            IsGraduated = from.GraduatedAt is not null,
        };
    }
}