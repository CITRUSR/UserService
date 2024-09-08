using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("UserService.Tests.AssemblyInitializer", "UserService.Tests")]

namespace UserService.Tests;

public class AssemblyInitializer : XunitTestFramework
{
    public AssemblyInitializer(IMessageSink messageSink)
        : base(messageSink) { }
}
