using UserService.Domain.Entities;

namespace UserService.API.Mappers;

public class SpeicalityViewModelMapper : IMapper<Speciality, SpecialityViewModel>
{
    public SpecialityViewModel Map(Speciality from)
    {
        return new SpecialityViewModel
        {
            Id = from.Id,
            Name = from.Name,
            Abbreviation = from.Abbreviation,
        };
    }
}
