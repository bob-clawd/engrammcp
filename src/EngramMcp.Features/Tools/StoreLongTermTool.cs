using System.ComponentModel;
using EngramMcp.Core.Abstractions;
using ModelContextProtocol.Server;

namespace EngramMcp.Features.Tools;

public sealed class StoreLongTermTool(IMemoryService memoryService) : Tool
{
    private const string MemoryName = "long-term";

    [McpServerTool(Name = "store_longterm", Title = "Store Long-Term Memory")]
    [Description("Use this tool for stable, remember-worthy facts about the human or yourself - such as identity, preferences, or interaction style - that should persist indefinitely. Capacity: 40 memories. FIFO.")]
    public Task ExecuteAsync(
        [Description("The memory to store.")]
        string text,
        CancellationToken cancellationToken)
    {
        return memoryService.StoreAsync(MemoryName, text, cancellationToken);
    }
}
