using System.ComponentModel;
using EngramMcp.Tools.Memory;
using ModelContextProtocol.Server;

namespace EngramMcp.Tools.Tools;

public sealed class RememberLongTool(MemoryService memories) : Tool
{
    [McpServerTool(Name = "remember_long", Title = "Remember Long")]
    [Description("Store information expected to remain valid over long periods. Use for durable facts, stable constraints, or information with low expected change frequency.")]
    public async Task<string?> ExecuteAsync(
        [Description("The memory to store.")]
        string text,
        CancellationToken cancellationToken = default)
    {
        var result = await memories.RememberAsync(RetentionTier.Long, text, cancellationToken).ConfigureAwait(false);
        return result.Rejection;
    }
}
