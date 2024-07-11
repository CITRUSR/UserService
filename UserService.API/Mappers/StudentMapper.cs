using Google.Protobuf.WellKnownTypes;

namespace UserService.Persistance.Mappers;

public class StudentMapper : IMapper<Domain.Entities.Student, StudentModel>
{
    public StudentModel Map(Domain.Entities.Student from)
    {
        return new StudentModel
        {
            Id = from.Id.ToString(),
            SsoId = from.SsoId.ToString(),
            FistName = from.FirstName,
            LastName = from.LastName,
            PatronymicName = from.PatronymicName,
            GroupId = from.GroupId,
            IsDropped = from.DroppedOutAt is not null,
            DroppedTime = Timestamp.FromDateTime(from.DroppedOutAt.Value)
        };
    }
}