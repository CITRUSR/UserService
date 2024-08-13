using UserService.Domain.Entities;

namespace UserService.Tests.Common.Customizations;

public class StudentCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Student>(composer => composer.Without(x => x.Id));
    }
}
