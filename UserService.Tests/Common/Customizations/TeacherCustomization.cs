using UserService.Domain.Entities;

namespace UserService.Tests.Common.Customizations;

public class TeacherCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Teacher>(composer => composer.Without(x => x.Id));
    }
}
