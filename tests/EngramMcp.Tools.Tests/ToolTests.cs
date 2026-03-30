using EngramMcp.Tools.Memory;
using EngramMcp.Tools.Tests.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace EngramMcp.Tools.Tests;

public abstract class ToolTests<TTool> : IDisposable where TTool : notnull
{
    protected ServiceProvider ServiceProvider { get; }

    protected TTool Sut { get; }

    protected ToolTestMemoryService MemoryService { get; }

    protected ToolTests()
    {
        MemoryService = new ToolTestMemoryService();

        ServiceProvider = new ServiceCollection()
            .WithEngramMcp(Path.Combine(Path.GetTempPath(), "engram-mcp-tools-tests", "memory.json"))
            .AddSingleton(MemoryService)
            .AddSingleton<IMemoryService>(MemoryService)
            .BuildServiceProvider();

        Sut = ServiceProvider.GetRequiredService<TTool>();
    }

    public void Dispose() => ServiceProvider.Dispose();
}
