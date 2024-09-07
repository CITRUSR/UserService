using UserService.Domain.Entities;

namespace UserService.API.Mappers;

public class ChangeGroupResponseModelMapper : IMapper<Group, ChangeGroupResponseModel>
{
    public ChangeGroupResponseModel Map(Group from)
    {
        return new ChangeGroupResponseModel
        {
            Abbr = from.Speciality.Abbreviation,
            CurrentCourse = from.CurrentCourse,
            SubGroup = from.SubGroup,
            Id = from.Id,
        };
    }
}
