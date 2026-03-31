using EngramMcp.Tools.Memory;
using EngramMcp.Tools.Memory.Storage;
using EngramMcp.Tools.Tools;
using Is.Assertions;
using Xunit;

namespace EngramMcp.Tools.Tests.Tools;

public sealed class RecallToolTests : ToolTests<RecallTool>
{
    [Fact]
    public async Task ExecuteAsync_returns_memories_from_service()
    {
        Store.Replace(new PersistedMemoryDocument
        {
            Memories =
            [
                new PersistedMemory { Id = "id-1", Text = "Remember this", Retention = 10 }
            ]
        });

        var response = await Sut.ExecuteAsync();

        response.Memories.Count.Is(1);
        response.Memories[0].Id.Is("id-1");
        response.Memories[0].Text.Is("Remember this");
    }

    [Fact]
    public async Task ExecuteAsync_caps_returned_memories_at_100()
    {
        Store.Replace(new PersistedMemoryDocument
        {
            Memories = Enumerable.Range(1, 101)
                .Select(index => new PersistedMemory { Id = $"id-{index}", Text = $"Memory {index}", Retention = 102 - index })
                .ToList()
        });

        var response = await Sut.ExecuteAsync();

        response.Memories.Count.Is(100);
        response.Memories[0].Id.Is("id-1");
        response.Memories[99].Id.Is("id-100");
    }
}
