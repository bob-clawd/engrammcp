using EngramMcp.Tools.Memory;
using EngramMcp.Tools.Memory.Storage;
using EngramMcp.Tools.Tests.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace EngramMcp.Tools.Tests;

public abstract class ToolTests<TTool> : IDisposable where TTool : notnull
{
    protected ServiceProvider ServiceProvider { get; }

    protected TTool Sut { get; }

    protected InMemoryMemoryStore Store { get; }

    protected ToolTests()
    {
        Store = new InMemoryMemoryStore(new PersistedMemoryDocument());

        ServiceProvider = new ServiceCollection()
            .WithEngramMcp(Path.Combine(Path.GetTempPath(), "engram-mcp-tools-tests", "memory.json"))
            .AddSingleton<IMemoryStore>(Store)
            .BuildServiceProvider();

        Sut = ServiceProvider.GetRequiredService<TTool>();
    }

    public void Dispose() => ServiceProvider.Dispose();
}
