using Is.Assertions;
using Xunit;
using Xunit.Abstractions;

namespace EngramMcp.Features.Tests.Inspections.Tools;

public sealed class DemoToolTests(ITestOutputHelper output)
{
    [Fact]
    public async Task Demo_Test()
    {
        (5 + 4).Is(9);
    }
}
