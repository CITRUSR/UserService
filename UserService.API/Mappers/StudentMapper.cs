using Google.Protobuf.WellKnownTypes;
using UserService.Domain.Entities;

namespace UserService.API.Mappers;

public class StudentMapper : IMapper<Student, StudentModel>
{
    public StudentModel Map(Student from)
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
