using EngramMcp.Tools.Memory.Storage;
using EngramMcp.Tools.Tools;
using Is.Assertions;
using Xunit;

namespace EngramMcp.Tools.Tests.Tools;

public sealed class ReinforceToolTests : ToolTests<ReinforceTool>
{
    [Fact]
    public async Task ExecuteAsync_reinforces_requested_memories()
    {
        Store.Replace(new PersistedMemoryDocument
        {
            Memories =
            [
                new PersistedMemory { Id = "id-1", Text = "First memory", Retention = 10 },
                new PersistedMemory { Id = "id-2", Text = "Second memory", Retention = 10 }
            ]
        });

        var response = await Sut.ExecuteAsync(["id-1", "id-2"]);

        response.IsNull();
        Store.Document.Memories.Single(memory => memory.Id == "id-1").Retention.Is(9.9d);
        Store.Document.Memories.Single(memory => memory.Id == "id-2").Retention.Is(9.9d);
    }

    [Fact]
    public async Task ExecuteAsync_returns_validation_message_for_invalid_input()
    {
        var response = await Sut.ExecuteAsync([]);

        response.Is("At least one memory id is required.");
    }
}
