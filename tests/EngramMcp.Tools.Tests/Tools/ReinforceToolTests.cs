using Is.Assertions;
using Xunit;

namespace EngramMcp.Tools.Tests.Tools;

public sealed class ReinforceToolTests
{
    [Fact]
    public async Task ExecuteAsync_reinforces_requested_memories()
    {
        var memoryService = new ToolTestMemoryService();
        var tool = new EngramMcp.Tools.Tools.Reinforce.McpTool(memoryService);

        var response = await tool.ExecuteAsync(["id-1", "id-2"]);

        response.Is("Reinforced memories.");
        memoryService.ReinforcedMemoryIds.IsNotNull();
        memoryService.ReinforcedMemoryIds!.Count.Is(2);
        memoryService.ReinforcedMemoryIds[0].Is("id-1");
        memoryService.ReinforcedMemoryIds[1].Is("id-2");
    }

    [Fact]
    public async Task ExecuteAsync_returns_validation_message_for_invalid_input()
    {
        var memoryService = new ToolTestMemoryService
        {
            ReinforceException = new ArgumentException("At least one memory id is required.", "memoryIds")
        };
        var tool = new EngramMcp.Tools.Tools.Reinforce.McpTool(memoryService);

        var response = await tool.ExecuteAsync([]);

        response.Is("At least one memory id is required. (Parameter 'memoryIds')");
    }
}
