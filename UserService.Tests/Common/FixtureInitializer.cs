using UserService.Tests.Common.Customizations;

namespace UserService.Tests.Common;

public class FixtureInitializer
{
    public static void Initialize(IFixture fixture)
    {
        fixture.Customize(new GroupCustomization());
        fixture.Customize(new SpecialityCustomization());
        fixture.Customize(new StudentCustomization());
        fixture.Customize(new TeacherCustomization());
    }
}
