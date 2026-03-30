using EngramMcp.Tools.Memory;
using EngramMcp.Tools.Tools;
using Is.Assertions;
using Xunit;

namespace EngramMcp.Tools.Tests.Tools;

public sealed class ReinforceToolTests : ToolTests<ReinforceTool>
{
    [Fact]
    public async Task ExecuteAsync_reinforces_requested_memories()
    {
        var response = await Sut.ExecuteAsync(["id-1", "id-2"]);

        response.Is("Reinforced memories.");
        MemoryService.ReinforcedMemoryIds.IsNotNull();
        MemoryService.ReinforcedMemoryIds!.Count.Is(2);
        MemoryService.ReinforcedMemoryIds[0].Is("id-1");
        MemoryService.ReinforcedMemoryIds[1].Is("id-2");
    }

    [Fact]
    public async Task ExecuteAsync_returns_validation_message_for_invalid_input()
    {
        MemoryService.ReinforceResult = MemoryChangeResult.Reject("At least one memory id is required.");

        var response = await Sut.ExecuteAsync([]);

        response.Is("At least one memory id is required.");
    }
}
