using UserService.Domain.Entities;

namespace UserService.Tests.Common.Customizations;

public class GroupCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Group>(composer => composer.Without(x => x.Id));
    }
}
