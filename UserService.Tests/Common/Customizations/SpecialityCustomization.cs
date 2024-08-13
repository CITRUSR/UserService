using UserService.Domain.Entities;

namespace UserService.Tests.Common.Customizations;

public class SpecialityCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Speciality>(composer => composer.Without(x => x.Id));
    }
}
