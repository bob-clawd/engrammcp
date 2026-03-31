using System.ComponentModel;
using EngramMcp.Tools.Memory;
using ModelContextProtocol.Server;

namespace EngramMcp.Tools.Tools;

public sealed class RememberShortTool(MemoryService memories) : Tool
{
    [McpServerTool(Name = "remember_short", Title = "Remember Short")]
    [Description("Store session-level context that helps future continuation. Use for recent progress, temporary working context, intermediate conclusions, or resume points.")]
    public async Task<string?> ExecuteAsync(
        [Description("The memory to store.")]
        string text,
        CancellationToken cancellationToken = default)
    {
        var result = await memories.RememberAsync(RetentionTier.Short, text, cancellationToken).ConfigureAwait(false);
        return result.Rejection;
    }
}
