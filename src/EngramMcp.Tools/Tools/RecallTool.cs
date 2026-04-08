using System.ComponentModel;
using EngramMcp.Tools.Memory;
using ModelContextProtocol.Server;

namespace EngramMcp.Tools.Tools;

public sealed record RecallResponse(
    IReadOnlyList<RecallMemory> Memories);

public sealed class RecallTool(MemoryService memories) : Tool
{
    private const int DefaultReturnedMemoryCount = 50;

    [McpServerTool(Name = "recall", Title = "Recall Memories")]
    [Description("Load the strongest current memories. Useful at the start of a session. Returns up to 50 memories.")]
    public async Task<RecallResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var recalledMemories = await memories.RecallAsync(cancellationToken).ConfigureAwait(false);

        var selected = recalledMemories.Take(DefaultReturnedMemoryCount).ToArray();
        return new RecallResponse(selected);
    }
}
