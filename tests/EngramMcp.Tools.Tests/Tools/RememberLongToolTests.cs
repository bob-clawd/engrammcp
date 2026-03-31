using EngramMcp.Tools.Tools;
using Is.Assertions;
using Xunit;

namespace EngramMcp.Tools.Tests.Tools;

public sealed class RememberLongToolTests : ToolTests<RememberLongTool>
{
    [Fact]
    public async Task ExecuteAsync_stores_long_term_memory()
    {
        var response = await Sut.ExecuteAsync("Remember this");

        response.IsNull();
        Store.Document.Memories.Count.Is(1);
        Store.Document.Memories[0].Text.Is("Remember this");
        Store.Document.Memories[0].Retention.Is(100d);
    }

    [Fact]
    public async Task ExecuteAsync_returns_validation_message_from_memory_service()
    {
        var response = await Sut.ExecuteAsync("");

        response.Is("Memory text must not be null, empty, or whitespace.");
        Store.Document.Memories.IsEmpty();
    }
}
