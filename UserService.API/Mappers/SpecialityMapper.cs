using UserService.Domain.Entities;

namespace UserService.API.Mappers;

public class SpecialityMapper : IMapper<Speciality, SpecialityModel>
{
    public SpecialityModel Map(Speciality from)
    {
        CustomTypes.DecimalValue constDec = from.Cost;
        var cost = new DecimalValue
        {
            Units = constDec.Units,
            Nanos = constDec.Nanos
        };
        return new SpecialityModel
        {
            Id = from.Id,
            Name = from.Name,
            Abbreavation = from.Abbreavation,
            Cost = cost,
            DurationMonths = from.DurationMonths,
            IsDeleted = from.IsDeleted,
        };
    }
}