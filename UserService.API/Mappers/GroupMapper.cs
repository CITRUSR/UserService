using Google.Protobuf.WellKnownTypes;

namespace UserService.Persistance.Mappers;

public class GroupMapper : IMapper<Domain.Entities.Group, GroupModel>
{
    public GroupModel Map(Domain.Entities.Group from)
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